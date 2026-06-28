using System;
using System.Collections.Generic;
using Celeste.Components;
using QC.Match.Commands;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.AI
{
    public static class AlphaBeta
    {
        private class Board : IChessBoardLookup
        {
           public readonly Dictionary<Vector2Int, Piece> Pieces = new();
           public readonly Move Move;
           
           private readonly ChessBoard chessBoard;

           public Board(ChessBoard chessBoard)
           {
              this.chessBoard = chessBoard;

              foreach (var chessBoardObject in chessBoard.Objects)
              {
                 if (chessBoardObject.TryFindComponent<IGrid2DTransform>(out var transform) &&
                     chessBoardObject.TryFindComponent<IChessPiece>(out var chessPiece) &&
                     chessBoardObject.TryFindComponent<IActionProvider>(out var actionProvider))
                 {
                    Vector2Int piecePosition = transform.iFace.GetPosition(transform.instance);
                    Pieces[piecePosition] = new Piece(chessBoardObject, chessPiece, actionProvider);
                 }
              }
           }

           private Board(Board board, Move move)
           {
              chessBoard = board.chessBoard;
              
              Move = move;

              foreach (var piece in board.Pieces)
              {
                 Pieces[piece.Key] = piece.Value;
              }
           }
           
           public bool DoesChessBoardTileExist(Vector2Int boardPosition)
           {
              return chessBoard.DoesChessBoardTileExist(boardPosition);
           }

           public ChessBoardObjectInstance GetChessBoardObjectAt(Vector2Int boardPosition)
           {
              return Pieces.TryGetValue(boardPosition, out var piece) ? piece.Instance : null;
           }

           public void RemovePieceAt(Vector2Int position)
           {
              Pieces.Remove(position);
           }

           public void ReplacePieceAt(Vector2Int position, Piece piece)
           {
              Pieces[position] = piece;
           }

           public Board MakeMoveWithPiece(Move moveToMake)
           {
              Board boardWithMove = new Board(this, moveToMake);
              boardWithMove.Pieces.Remove(moveToMake.Position);
              boardWithMove.Pieces[moveToMake.Destination] = moveToMake.Piece;

              return boardWithMove;
           }
        }

        private readonly struct Piece
        {
           public bool IsValid => Instance != null && ChessPiece.IsValid && ActionProvider.IsValid;
           
           public readonly ChessBoardObjectInstance Instance;
           public readonly InterfaceHandle<IChessPiece> ChessPiece;
           public readonly InterfaceHandle<IActionProvider> ActionProvider;

           public Piece(
              ChessBoardObjectInstance instance, 
              InterfaceHandle<IChessPiece> chessPiece,
              InterfaceHandle<IActionProvider> actionProvider)
           {
              Instance = instance;
              ChessPiece = chessPiece;
              ActionProvider = actionProvider;
           }
        }

        private readonly struct Move
        {
           public bool IsValid => Position != Destination && Piece.IsValid;
           
           public readonly Piece Piece;
           public readonly Vector2Int Position;
           public readonly Vector2Int Destination;

           public Move(Piece piece, Vector2Int position, Vector2Int destination)
           {
              Piece = piece;
              Position = position;
              Destination = destination;
           }
        }
        
        public static bool TryCalculate(
            ITurnStrategy.Args args,
            int depth,
            out ICommand<ChessBoardCommandArgs> command)
        {
           Board board = new Board(args.ChessBoard);
           var bestMove = AlphaBetaMax(board, args.MatchState.ActiveColour, int.MinValue, int.MaxValue, depth);

           if (!bestMove.Item2.IsValid)
           {
              UnityEngine.Debug.LogAssertion($"{nameof(AlphaBetaMax)} failed to find any valid moves...");
              command = null;
              return false;
           }
           
           var objectAtBestMove = args.ChessBoard.GetChessBoardObjectAt(bestMove.Item2.Destination);

           if (objectAtBestMove != null)
           {
              // Our best move involves an attack
              // We run a few basic checks first to ensure the algorithm hasn't done something weird
              UnityEngine.Debug.Assert(args.ChessBoard.GetChessBoardObjectAt(bestMove.Item2.Position) != null, 
                 $"Attempting to attack with a piece at {bestMove.Item2.Position}, but there is no object there!");
              UnityEngine.Debug.Assert(objectAtBestMove.TryFindComponent<IChessPiece>(out var pieceToTake), 
                 $"Failed to find a {nameof(IChessPiece)} interface on a piece that we have decided to take.");
              UnityEngine.Debug.Assert(pieceToTake.iFace.GetPieceColour(pieceToTake.instance) != args.MatchState.ActiveColour, 
                 $"We're attempting to move on top of a piece at {bestMove.Item2.Destination} of the same colour.  Something has gone wrong with the AI here...");
              command = new AttackWithChessBoardObjectFromToCommand(bestMove.Item2.Position, bestMove.Item2.Destination);
           }
           else
           {
              // Our best move involves a move
              // We run a few basic checks first to ensure the algorithm hasn't done something weird
              UnityEngine.Debug.Assert(args.ChessBoard.GetChessBoardObjectAt(bestMove.Item2.Position) != null, 
                 $"Attempting to move a piece at {bestMove.Item2.Position}, but there is no object there!");
              command = new MoveChessBoardObjectFromToCommand(bestMove.Item2.Position, bestMove.Item2.Destination);
           }

           return true;
        }

        private static ValueTuple<int, Move> AlphaBetaMax(
           Board board, 
           ChessPieceColour activeColour, 
           int alpha, 
           int beta, 
           int remainingDepth)
        {
           if (remainingDepth == 0)
           {
              int currentBoardScore = EvaluateBoard(board, activeColour);
              return (currentBoardScore, board.Move);
           }
           
           ValueTuple<int, Move> bestMove = new ValueTuple<int, Move>(int.MinValue, default);
           List<Vector2Int> validActions = new List<Vector2Int>();

           foreach (var piece in board.Pieces)
           {
              var chessPiece = piece.Value.ChessPiece;
              if (chessPiece.iFace.GetPieceColour(chessPiece.instance) != activeColour)
              {
                 continue;
              }
              
              validActions.Clear();
              
              var actionProvider = piece.Value.ActionProvider;
              actionProvider.iFace.CalculateValidAttacks(actionProvider.instance, piece.Key, board, validActions);
              actionProvider.iFace.CalculateValidMoves(actionProvider.instance, piece.Key, board, validActions);

              foreach (var action in validActions)
              {
                 // Need to make a new representation of the board here taking into account the action
                 Board nextBoard = board.MakeMoveWithPiece(new Move(piece.Value, piece.Key, action));
                 var scoreAndMove = AlphaBetaMin(nextBoard, activeColour, alpha, beta, remainingDepth - 1);
                 int score = scoreAndMove.Item1;
                 
                 if (score > bestMove.Item1)
                 {
                    bestMove = scoreAndMove;

                    if (score > alpha)
                    {
                       alpha = score;
                    }
                 }

                 if (score >= beta)
                 {
                    return scoreAndMove;
                 }
              }
           }
           
           return bestMove;
        }

        private static ValueTuple<int, Move> AlphaBetaMin(
           Board board, 
           ChessPieceColour activeColour, 
           int alpha, 
           int beta, 
           int remainingDepth)
        {
           if (remainingDepth == 0)
           {
              int currentBoardScore = EvaluateBoard(board, activeColour);
              return (currentBoardScore, board.Move);
           }
           
           ValueTuple<int, Move> bestMove = new ValueTuple<int, Move>(int.MaxValue, default);
           List<Vector2Int> validActions = new List<Vector2Int>();

           foreach (var piece in board.Pieces)
           {
              var chessPiece = piece.Value.ChessPiece;
              if (chessPiece.iFace.GetPieceColour(chessPiece.instance) == activeColour)
              {
                 continue;
              }
              
              validActions.Clear();
              
              var actionProvider = piece.Value.ActionProvider;
              actionProvider.iFace.CalculateValidAttacks(actionProvider.instance, piece.Key, board, validActions);
              actionProvider.iFace.CalculateValidMoves(actionProvider.instance, piece.Key, board, validActions);

              foreach (var action in validActions)
              {
                 // Need to make a new representation of the board here taking into account the action
                 Board nextBoard = board.MakeMoveWithPiece(new Move(piece.Value, piece.Key, action));
                 var scoreAndMove = AlphaBetaMax(nextBoard, activeColour, alpha, beta, remainingDepth - 1);
                 int score = scoreAndMove.Item1;
                 
                 if (score < bestMove.Item1)
                 {
                    bestMove = scoreAndMove;

                    if (score < beta)
                    {
                       beta = score;
                    }
                 }

                 if (score <= alpha)
                 {
                    return scoreAndMove;
                 }
              }
           }
           
           return bestMove;
        }

        private static int EvaluateBoard(
           Board board,
           ChessPieceColour activeColour)
        {
           int total = 0;

           foreach (var piece in board.Pieces)
           {
              var chessPieceComponent = piece.Value.ChessPiece;
              int materialWorth = CalculateMaterialWorth(chessPieceComponent.iFace.GetPieceType(chessPieceComponent.instance));
              int colourModifier = chessPieceComponent.iFace.GetPieceColour(chessPieceComponent.instance) == activeColour ? 1 : -1;
              
              total += materialWorth * colourModifier;
           }
           
           // Can adjust this with extra bits and bobs, e.g. a board heatmap, promotion zones (a specific instance of a board heatmap I suppose) and other things

           return total;
        }

        private static int CalculateMaterialWorth(ChessPieceType chessPieceType)
        {
           switch (chessPieceType)
           {
              case ChessPieceType.King:
                 return 900;
              
              case ChessPieceType.Queen:
                 return 9;
              
              case ChessPieceType.Rook:
                 return 5;
              
              case ChessPieceType.Bishop:
                 return 3;
              
              case ChessPieceType.Knight:
                 return 3;
              
              case ChessPieceType.Pawn:
                 return 1;
              
              case ChessPieceType.None:
              default:
                 return 0;
           }
        }
    }
}