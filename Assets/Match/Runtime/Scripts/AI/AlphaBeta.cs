using System;
using System.Collections.Generic;
using Celeste.Components;
using Celeste.Core.Interfaces;
using QC.Match.Commands;
using QC.Match.Objects;
using UnityEngine;
using UnityEngine.Pool;

namespace QC.Match.AI
{
    public static class AlphaBeta
    {
        private class Board : IChessBoardLookup
        {
           public Dictionary<Vector2Int, Piece> Pieces => pieces;
           
           private readonly ChessBoard chessBoard;
           private readonly Dictionary<Vector2Int, Piece> pieces = new(32);

           public Board(ChessBoard chessBoard)
           {
              this.chessBoard = chessBoard;
              pieces.EnsureCapacity(chessBoard.Objects.Count);

              foreach (var chessBoardObject in chessBoard.Objects)
              {
                 if (chessBoardObject.TryFindComponent<IGrid2DTransform>(out var transform) &&
                     chessBoardObject.TryFindComponent<IChessPiece>(out var chessPiece) &&
                     chessBoardObject.TryFindComponent<IActionProvider>(out var actionProvider))
                 {
                    Vector2Int piecePosition = transform.GetPosition();
                    pieces[piecePosition] = new Piece(chessBoardObject, chessPiece, actionProvider);
                 }
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

           public int GetTileImportance(Vector2Int boardPosition)
           {
              return chessBoard.GetTileImportance(boardPosition);
           }

           public void MakeMove(Move moveToMake)
           {
              pieces.Remove(moveToMake.Position);
              pieces[moveToMake.Destination] = moveToMake.PieceToMove;
           }

           public void UndoMove(Move moveToUndo)
           {
              if (moveToUndo.PieceToCapture.IsValid)
              {
                 pieces[moveToUndo.Destination] = moveToUndo.PieceToCapture;
              }
              else
              {
                 pieces.Remove(moveToUndo.Destination);
              }
              
              pieces[moveToUndo.Position] = moveToUndo.PieceToMove;
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

           public Piece(ChessBoardObjectInstance instance) : 
              this(instance, instance.FindInterface<IChessPiece>(), instance.FindInterface<IActionProvider>())
           {
           }
        }

        private readonly struct Move
        {
           public bool IsValid => Position != Destination && PieceToMove.IsValid;
           
           public readonly Piece PieceToMove;
           public readonly Piece PieceToCapture;
           public readonly Vector2Int Position;
           public readonly Vector2Int Destination;

           public Move(Piece pieceToMove, Piece pieceToCapture, Vector2Int position, Vector2Int destination)
           {
              PieceToMove = pieceToMove;
              PieceToCapture = pieceToCapture;
              Position = position;
              Destination = destination;
           }
        }

        private const int k_defaultValidMovesAllocation = 20;
        private const int k_defaultValidMovesPoolCapacity = 1000;
        
        private static ObjectPool<List<Vector2Int>> s_validMovesPool = new(
           () => new List<Vector2Int>(k_defaultValidMovesAllocation),
           actionOnRelease: l => l.Clear(),
           defaultCapacity: k_defaultValidMovesPoolCapacity);
        
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
           
           using (AlphaBetaProfileMarkers.MainLoop.Auto())
           {
              ChessPieceColour activeColour = args.MatchState.ActiveColour;
              Move bestMove = default;
              int bestScore = int.MinValue;
              int alpha = int.MinValue;
              int beta = int.MaxValue;
              Board board = new Board(args.ChessBoard);
              
              foreach (var pieceInstance in piecesToConsider)
              {
                 var chessPiece = pieceInstance.FindInterface<IChessPiece>();
                 if (chessPiece.GetPieceColour() != activeColour)
                 {
                    continue;
                 }

                 List<Vector2Int> validActions = s_validMovesPool.Get();

                 var actionProvider = pieceInstance.FindInterface<IActionProvider>();
                 var transform = pieceInstance.FindInterface<IGrid2DTransform>();
                 Vector2Int position = transform.GetPosition();

                 actionProvider.CalculateValidAttacks(position, args.ChessBoard, validActions);
                 int numAttacks = validActions.Count;
                 actionProvider.CalculateValidMoves(position, args.ChessBoard, validActions);

                 for (int i = 0, n = validActions.Count; i < n; ++i)
                 {
                    Vector2Int action = validActions[i];
                    Piece piece = new Piece(pieceInstance, chessPiece, actionProvider);
                    Move move;

                    if (i < numAttacks)
                    {
                       ChessBoardObjectInstance pieceToCaptureInstance = args.ChessBoard.GetChessBoardObjectAt(action);
                       Piece pieceToCapture = new Piece(pieceToCaptureInstance);
                       move = new Move(piece, pieceToCapture, position, action);
                    }
                    else
                    {
                       move = new Move(piece, default, position, action);
                    }
                    
                    int score = 0;

                    board.MakeMove(move);
                    score = AlphaBetaMin(alphaBetaParameters, board, activeColour, args.RNG, alpha, beta,
                       alphaBetaParameters.Depth - 1);
                    board.UndoMove(move);

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
                 
                 s_validMovesPool.Release(validActions);

                 if (beta <= alpha)
                 {
                    break;
                 }
              }
              
              if (!bestMove.IsValid)
              {
                 UnityEngine.Debug.LogAssertion(
                    $"{nameof(AlphaBeta)} failed to find any valid moves (possibly Checkmate or Stalemate).");
                 command = null;
                 return false;
              }

              var bestMoveChessPiece = bestMove.PieceToMove.ChessPiece;
              UnityEngine.Debug.Assert(bestMoveChessPiece.GetPieceColour() == activeColour,
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
                 UnityEngine.Debug.Assert(pieceToTake.GetPieceColour() != args.MatchState.ActiveColour,
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
           }

           return true;
        }

        private static int AlphaBetaMax(
           AlphaBetaParameters parameters,
           Board board, 
           ChessPieceColour activeColour,
           IRNG rng,
           int alpha, 
           int beta,
           int remainingDepth)
        {
           if (remainingDepth == 0)
           {
              int currentBoardScore = EvaluateBoard(parameters, board, activeColour, rng);
              return currentBoardScore;
           }

           int maxValue = int.MinValue;
           
           foreach (var piece in board.Pieces)
           {
              var chessPiece = piece.Value.ChessPiece;
              if (chessPiece.GetPieceColour() != activeColour)
              {
                 continue;
              }
              
              List<Vector2Int> validActions = s_validMovesPool.Get();
              
              var actionProvider = piece.Value.ActionProvider;
              
              actionProvider.CalculateValidAttacks(piece.Key, board, validActions);
              int numAttacks = validActions.Count;
              actionProvider.CalculateValidMoves(piece.Key, board, validActions);

              for (int i = 0, n = validActions.Count; i < n; ++i)
              {
                 Vector2Int action = validActions[i];
                 Move move;

                 if (i < numAttacks)
                 {
                    ChessBoardObjectInstance pieceToCaptureInstance = board.GetChessBoardObjectAt(action);
                    Piece pieceToCapture = new Piece(pieceToCaptureInstance);
                    move = new Move(piece.Value, pieceToCapture, piece.Key, action);
                 }
                 else
                 {
                    move = new Move(piece.Value, default, piece.Key, action);
                 }
                 
                 int score = 0;

                 board.MakeMove(move);
                 score = AlphaBetaMin(parameters, board, activeColour, rng, alpha, beta, remainingDepth - 1);
                 board.UndoMove(move);

                 maxValue = Math.Max(maxValue, score);
                 alpha = Math.Max(alpha, score);

                 if (beta <= alpha)
                 {
                    s_validMovesPool.Release(validActions);
                    return maxValue;
                 }
              }
              
              s_validMovesPool.Release(validActions);
           }
           
           return maxValue;
        }

        private static int AlphaBetaMin(
           AlphaBetaParameters parameters,
           Board board,
           ChessPieceColour activeColour,
           IRNG rng,
           int alpha, 
           int beta, 
           int remainingDepth)
        {
           if (remainingDepth == 0)
           {
              int currentBoardScore = EvaluateBoard(parameters, board, activeColour, rng);
              return currentBoardScore;
           }
           
           int minValue = int.MaxValue;
           
           foreach (var piece in board.Pieces)
           {
              var chessPiece = piece.Value.ChessPiece;
              if (chessPiece.GetPieceColour() == activeColour)
              {
                 continue;
              }
              
              List<Vector2Int> validActions = s_validMovesPool.Get();

              var actionProvider = piece.Value.ActionProvider;
              
              actionProvider.CalculateValidAttacks(piece.Key, board, validActions);
              int numAttacks = validActions.Count;
              actionProvider.CalculateValidMoves(piece.Key, board, validActions);

              for (int i = 0, n = validActions.Count; i < n; ++i)
              {
                 Vector2Int action = validActions[i];
                 Move move;

                 if (i < numAttacks)
                 {
                    ChessBoardObjectInstance pieceToCaptureInstance = board.GetChessBoardObjectAt(action);
                    Piece pieceToCapture = new Piece(pieceToCaptureInstance);
                    move = new Move(piece.Value, pieceToCapture, piece.Key, action);
                 }
                 else
                 {
                    move = new Move(piece.Value, default, piece.Key, action);
                 }
                 
                 int score = 0;

                 board.MakeMove(move);
                 score = AlphaBetaMax(parameters, board, activeColour, rng, alpha, beta, remainingDepth - 1);
                 board.UndoMove(move);

                 minValue = Math.Min(minValue, score);
                 beta = Math.Min(beta, score);

                 if (beta <= alpha)
                 {
                    s_validMovesPool.Release(validActions);
                    return minValue;
                 }
              }
              
              s_validMovesPool.Release(validActions);
           }
           
           return minValue;
        }

        private static int EvaluateBoard(
           AlphaBetaParameters parameters,
           Board board,
           ChessPieceColour activeColour,
           IRNG rng)
        {
           int total = 0;

           foreach (var piece in board.Pieces)
           {
              int materialWorth = CalculateValue(
                 parameters,
                 piece.Key, 
                 piece.Value, 
                 activeColour, 
                 board,
                 rng);
              
              total += materialWorth;
           }
           
           return total;
        }

        private static int CalculateValue(
           AlphaBetaParameters parameters,
           Vector2Int position,
           Piece piece,
           ChessPieceColour activeColour,
           IChessBoardLookup boardLookup,
           IRNG rng)
        {
           int colourModifier = piece.ChessPiece.GetPieceColour() == activeColour ? 1 : -1;
           int chessPieceValue = piece.ChessPiece.GetValue();
           ChessPieceType chessPieceType = piece.ChessPiece.GetPieceType();
           int chessPieceValueOverride = parameters.GetValueOverrideForPiece(chessPieceType, rng);
           int positionImportance = boardLookup.GetTileImportance(position);
           int positionImportanceOverride = parameters.GetImportanceOverrideForPosition(position);
           
           int value = chessPieceValue + chessPieceValueOverride + positionImportance + positionImportanceOverride;
           return value * colourModifier;
        }
    }
}