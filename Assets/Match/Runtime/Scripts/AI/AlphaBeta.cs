using System;
using System.Collections.Generic;
using System.Linq;
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
           
           private readonly Move move;
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
                    Vector2Int piecePosition = transform.GetPosition();
                    Pieces[piecePosition] = new Piece(chessBoardObject, chessPiece, actionProvider);
                 }
              }
           }

           private Board(Board board, Move move)
           {
              this.move = move;
              chessBoard = board.chessBoard;

              foreach (var piece in board.Pieces)
              {
                 Pieces[piece.Key] = piece.Value;
              }
              
              Pieces.Remove(move.Position);
              Pieces[move.Destination] = move.Piece;
           }
           
           public bool DoesChessBoardTileExist(Vector2Int boardPosition)
           {
              return chessBoard.DoesChessBoardTileExist(boardPosition);
           }

           public ChessBoardObjectInstance GetChessBoardObjectAt(Vector2Int boardPosition)
           {
              return Pieces.TryGetValue(boardPosition, out var piece) ? piece.Instance : null;
           }

           public int GetTileImportance(Vector2Int boardPosition)
           {
              return chessBoard.GetTileImportance(boardPosition);
           }

           public Board MakeMove(Move moveToMake)
           {
              Board boardWithMove = new Board(this, moveToMake);
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
            IReadOnlyList<ChessBoardObjectInstance> piecesToConsider,
            AlphaBetaParameters alphaBetaParameters,
            out ICommand<ChessBoardCommandArgs> command)
        {
           if (alphaBetaParameters.Depth < 1)
           {
              UnityEngine.Debug.LogAssertion($"Failed to calculate best move as calculate depth was insufficient - please provide a value of at least 1.");
              command = null;
              return false;
           }
           
           ChessPieceColour activeColour = args.MatchState.ActiveColour;
           Move bestMove = default;
           int bestScore = int.MinValue;
           int alpha = int.MinValue;
           int beta = int.MaxValue;

           List<Vector2Int> validActions = new List<Vector2Int>();
           
           foreach (var pieceInstance in piecesToConsider)
           {
              var chessPiece = pieceInstance.FindInterface<IChessPiece>();
              if (chessPiece.GetColour() != activeColour)
              {
                 continue;
              }

              validActions.Clear();
              
              var actionProvider = pieceInstance.FindInterface<IActionProvider>();
              var transform = pieceInstance.FindInterface<IGrid2DTransform>();
              Vector2Int position = transform.GetPosition();
              actionProvider.CalculateValidAttacks(position, args.ChessBoard, validActions);
              actionProvider.CalculateValidMoves(position, args.ChessBoard, validActions);

              foreach (var action in validActions)
              {
                 Piece piece = new Piece(pieceInstance, chessPiece, actionProvider);
                 Move move = new Move(piece, position, action);
                 Board nextBoard = new Board(args.ChessBoard).MakeMove(move);
         
                 int score = AlphaBetaMin(alphaBetaParameters, nextBoard, activeColour, alpha, beta, alphaBetaParameters.Depth - 1);
                 if (score > bestScore)
                 {
                    bestScore = score;
                    bestMove = move;

                    if (score > alpha)
                    {
                       alpha = score;
                    }
                 }
                 
                 if (beta <= alpha)
                 {
                    break;
                 }
              }
              
              if (beta <= alpha)
              {
                 break; 
              }
           }
           
           if (!bestMove.IsValid)
           {
              UnityEngine.Debug.LogAssertion($"{nameof(AlphaBeta)} failed to find any valid moves (possibly Checkmate or Stalemate).");
              command = null;
              return false;
           }

           var bestMoveChessPiece = bestMove.Piece.ChessPiece;
           UnityEngine.Debug.Assert(bestMoveChessPiece.GetColour() == activeColour, 
              $"Attempting to perform a move with a piece that is not the active colour: '{activeColour}'!");
           
           var objectAtBestMove = args.ChessBoard.GetChessBoardObjectAt(bestMove.Destination);
           if (objectAtBestMove != null)
           {
              // Our best move involves an attack
              // We run a few basic checks first to ensure the algorithm hasn't done something weird
              UnityEngine.Debug.Assert(args.ChessBoard.GetChessBoardObjectAt(bestMove.Position) != null, 
                 $"Attempting to attack with a piece at {bestMove.Position}, but there is no object there!");
              UnityEngine.Debug.Assert(objectAtBestMove.TryFindComponent<IChessPiece>(out var pieceToTake), 
                 $"Failed to find a {nameof(IChessPiece)} interface on a piece that we have decided to take.");
              UnityEngine.Debug.Assert(pieceToTake.GetColour() != args.MatchState.ActiveColour, 
                 $"We're attempting to move on top of a piece at {bestMove.Destination} of the same colour.  Something has gone wrong with the AI here...");
              command = new AttackWithChessBoardObjectFromToCommand(bestMove.Position, bestMove.Destination);
           }
           else
           {
              // Our best move involves a move
              // We run a few basic checks first to ensure the algorithm hasn't done something weird
              UnityEngine.Debug.Assert(args.ChessBoard.GetChessBoardObjectAt(bestMove.Position) != null, 
                 $"Attempting to move a piece at {bestMove.Position}, but there is no object there!");
              command = new MoveChessBoardObjectFromToCommand(bestMove.Position, bestMove.Destination);
           }

           return true;
        }

        private static int AlphaBetaMax(
           AlphaBetaParameters parameters,
           Board board, 
           ChessPieceColour activeColour, 
           int alpha, 
           int beta,
           int remainingDepth)
        {
           if (remainingDepth == 0)
           {
              int currentBoardScore = EvaluateBoard(board, activeColour, parameters);
              return currentBoardScore;
           }

           int maxValue = int.MinValue;
           List<Vector2Int> validActions = new List<Vector2Int>();

           foreach (var piece in board.Pieces)
           {
              var chessPiece = piece.Value.ChessPiece;
              if (chessPiece.GetColour() != activeColour)
              {
                 continue;
              }
              
              validActions.Clear();
              
              var actionProvider = piece.Value.ActionProvider;
              actionProvider.CalculateValidAttacks(piece.Key, board, validActions);
              actionProvider.CalculateValidMoves(piece.Key, board, validActions);

              foreach (var action in validActions)
              {
                 // Need to make a new representation of the board here taking into account the action
                 Move move = new Move(piece.Value, piece.Key, action);
                 Board nextBoard = board.MakeMove(move);
                 var score = AlphaBetaMin(parameters, nextBoard, activeColour, alpha, beta, remainingDepth - 1);
                 
                 maxValue = Math.Max(maxValue, score);
                 alpha = Math.Max(alpha, score);

                 if (beta <= alpha)
                 {
                    return maxValue;
                 }
              }
           }
           
           return maxValue;
        }

        private static int AlphaBetaMin(
           AlphaBetaParameters parameters,
           Board board, 
           ChessPieceColour activeColour, 
           int alpha, 
           int beta, 
           int remainingDepth)
        {
           if (remainingDepth == 0)
           {
              int currentBoardScore = EvaluateBoard(board, activeColour, parameters);
              return currentBoardScore;
           }
           
           int minValue = int.MaxValue;
           List<Vector2Int> validActions = new List<Vector2Int>();

           foreach (var piece in board.Pieces)
           {
              var chessPiece = piece.Value.ChessPiece;
              if (chessPiece.GetColour() == activeColour)
              {
                 continue;
              }
              
              validActions.Clear();
              
              var actionProvider = piece.Value.ActionProvider;
              actionProvider.CalculateValidAttacks(piece.Key, board, validActions);
              actionProvider.CalculateValidMoves(piece.Key, board, validActions);

              foreach (var action in validActions)
              {
                 // Need to make a new representation of the board here taking into account the action
                 Move move = new Move(piece.Value, piece.Key, action);
                 Board nextBoard = board.MakeMove(move);
                 int score = AlphaBetaMax(parameters, nextBoard, activeColour, alpha, beta, remainingDepth - 1);
                 
                 minValue = Math.Min(minValue, score);
                 beta = Math.Min(beta, score);

                 if (beta <= alpha)
                 {
                    return minValue;
                 }
              }
           }
           
           return minValue;
        }

        private static int EvaluateBoard(
           Board board,
           ChessPieceColour activeColour,
           AlphaBetaParameters parameters)
        {
           int total = 0;

           foreach (var piece in board.Pieces)
           {
              int materialWorth = CalculateValue(
                 piece.Key, 
                 piece.Value, 
                 activeColour, 
                 board,
                 parameters);
              
              total += materialWorth;
           }
           
           return total;
        }

        private static int CalculateValue(
           Vector2Int position,
           Piece piece,
           ChessPieceColour activeColour,
           IChessBoardLookup boardLookup,
           AlphaBetaParameters parameters)
        {
           int colourModifier = piece.ChessPiece.GetColour() == activeColour ? 1 : -1;
           int chessPieceValue = piece.ChessPiece.GetValue();
           int positionImportance = boardLookup.GetTileImportance(position);
           int positionImportanceOverride = parameters.GetImportanceOverrideForPosition(position);
           
           // Can adjust this with extra bits and bobs, e.g. a board heatmap, promotion zones (a specific instance of a board heatmap I suppose) and other things
           int value = chessPieceValue + positionImportance + positionImportanceOverride;
           return value * colourModifier;
        }
    }
}