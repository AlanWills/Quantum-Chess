using System;
using System.Collections.Generic;
using Celeste.Components;
using Celeste.Core.Interfaces;
using QC.Match.Commands;
using QC.Match.Objects;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Windows;

namespace QC.Match.AI
{
    public static class AlphaBeta
    {
        private class Board : IChessBoardLookup
        {
           // TODO: Oooffff, why public?
           // TODO: Can pool/cache this Tile[] for re-use between 
           public readonly Tile[] board;
           private readonly int width;
           private readonly int height;
           private readonly int xMin;
           private readonly int yMin;

           public Board(ChessBoard chessBoard, RectInt boardDimensions, Tile[] tileArray)
           {
              width = boardDimensions.size.x;
              height = boardDimensions.size.y;
              xMin = boardDimensions.xMin;
              yMin = boardDimensions.yMin;
              board = tileArray;
              
              foreach (var tile in chessBoard.Tiles)
              {
                 Vector2Int tilePosition = tile.Position;
                 int index = ToIndex(tilePosition);
                 ChessBoardObjectInstance chessBoardObjectInstance = chessBoard.GetChessBoardObjectAt(tilePosition);
                 Piece piece = chessBoardObjectInstance != null ? new Piece(chessBoardObjectInstance) : default;
                 UnityEngine.Debug.Assert(index < board.Length, $"Incorrectly calculated index ({index}) for position ({tilePosition}).");
                 board[index] = new Tile(true, chessBoard.GetTileImportance(tilePosition), piece);
              }
           }

           public int ToIndex(Vector2Int position)
           {
              int index = (position.x - xMin) + (position.y - yMin) * width;
              return index;
           }

           public Vector2Int ToVector2Int(int boardIndex)
           {
              return new Vector2Int((boardIndex % width) + xMin, (boardIndex / width) + yMin);
           }

           public Piece GetPieceAt(Vector2Int boardPosition)
           {
              int index = ToIndex(boardPosition);
              return board[index].Piece;
           }

           public void MakeMove(Move moveToMake)
           {
              int indexOfPosition = ToIndex(moveToMake.Position);
              int indexOfDestination = ToIndex(moveToMake.Destination);
              board[indexOfPosition] = board[indexOfPosition].WithoutPiece();
              board[indexOfDestination] = board[indexOfDestination].WithPiece(moveToMake.PieceToMove);
           }

           public void UndoMove(Move moveToUndo)
           {
              int indexOfPosition = ToIndex(moveToUndo.Position);
              int indexOfDestination = ToIndex(moveToUndo.Destination);
              board[indexOfPosition] = board[indexOfPosition].WithPiece(moveToUndo.PieceToMove);
              board[indexOfDestination] = board[indexOfPosition].WithPiece(moveToUndo.PieceToCapture);
           }

           public bool DoesChessBoardTileExist(Vector2Int boardPosition)
           {
              int index = ToIndex(boardPosition);
              return 0 <= index && index < board.Length && board[index].IsValid;
           }

           public ChessBoardObjectInstance GetChessBoardObjectAt(Vector2Int boardPosition)
           {
              int index = ToIndex(boardPosition);
              return 0 <= index && index < board.Length ? board[index].Piece.Instance : null;
           }

           public int GetTileImportance(Vector2Int boardPosition)
           {
              int index = ToIndex(boardPosition);
              return board[index].Importance;
           }
        }

        private readonly struct Tile
        {
           public readonly bool IsValid;
           public readonly int Importance;
           public readonly Piece Piece;

           public Tile(bool isValid, int importance) :
              this(isValid, importance, default)
           {
           }
           
           public Tile(bool isValid, int importance, Piece piece)
           {
              IsValid = isValid;
              Importance = importance;
              Piece = piece;
           }

           public Tile WithoutPiece()
           {
              return new Tile(IsValid, Importance);
           }
           
           public Tile WithPiece(Piece piece)
           {
              return new Tile(IsValid, Importance, piece);
           }
        }

        private readonly struct Piece
        {
           public bool IsValid => Instance != null && PieceType != ChessPieceType.None && PieceColour != ChessPieceColour.Unknown && ActionProvider.IsValid;
           
           public readonly ChessBoardObjectInstance Instance;
           public readonly ChessPieceType PieceType;
           public readonly ChessPieceColour PieceColour;
           public readonly int Value;
           public readonly InterfaceHandle<IActionProvider> ActionProvider;

           public Piece(
              ChessBoardObjectInstance instance, 
              InterfaceHandle<IChessPiece> chessPiece,
              InterfaceHandle<IActionProvider> actionProvider)
           {
              Instance = instance;
              PieceType = chessPiece.GetPieceType();
              PieceColour = chessPiece.GetPieceColour();
              Value = chessPiece.GetValue();
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
        private static Tile[] s_tiles;
        
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
              
              RectInt boundingBox = args.ChessBoard.CalculateBoundingBox();
              if ( s_tiles == null || s_tiles.Length != boundingBox.size.x * boundingBox.size.y)
              {
                 s_tiles = new Tile[boundingBox.size.x * boundingBox.size.y];
              }
              
              Board board = new Board(args.ChessBoard, boundingBox, s_tiles);
              
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
                 actionProvider.CalculateValidMoves(position, args.ChessBoard, validActions);

                 for (int i = 0, n = validActions.Count; i < n; ++i)
                 {
                    Vector2Int action = validActions[i];
                    Piece piece = new Piece(pieceInstance, chessPiece, actionProvider);
                    Piece pieceToCapture = board.GetPieceAt(action);
                    Move move = new Move(piece, pieceToCapture, position, action);

                    board.MakeMove(move);
                    int score = AlphaBetaMin(alphaBetaParameters, board, activeColour, args.RNG, alpha, beta,
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
              
              UnityEngine.Debug.Assert(bestMove.PieceToMove.PieceColour == activeColour,
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
           
           for (int boardIndex = 0, numBoardIndices = board.board.Length; boardIndex < numBoardIndices; ++boardIndex)
           {
              Tile tile = board.board[boardIndex];
              
              if (!tile.IsValid)
              {
                 continue;
              }
              
              if (!tile.Piece.IsValid)
              {
                 continue;
              }
              
              if (tile.Piece.PieceColour != activeColour)
              {
                 continue;
              }

              Vector2Int boardPosition = board.ToVector2Int(boardIndex);
              List<Vector2Int> validActions = s_validMovesPool.Get();
              
              var actionProvider = tile.Piece.ActionProvider;
              
              // TODO: May have to change these to support the faster AlphaBeta version
              actionProvider.CalculateValidAttacks(boardPosition, board, validActions);
              actionProvider.CalculateValidMoves(boardPosition, board, validActions);

              for (int i = 0, n = validActions.Count; i < n; ++i)
              {
                 Vector2Int action = validActions[i];
                 Piece pieceToCapture = board.GetPieceAt(action);
                 Move move = new Move(tile.Piece, pieceToCapture, boardPosition, action);
                 
                 board.MakeMove(move);
                 int score = AlphaBetaMin(parameters, board, activeColour, rng, alpha, beta, remainingDepth - 1);
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
           
           for (int boardIndex = 0, numBoardIndices = board.board.Length; boardIndex < numBoardIndices; ++boardIndex)
           {
              Tile tile = board.board[boardIndex];
              
              if (!tile.IsValid)
              {
                 continue;
              }
              
              if (!tile.Piece.IsValid)
              {
                 continue;
              }
              
              if (tile.Piece.PieceColour != activeColour)
              {
                 continue;
              }

              Vector2Int boardPosition = board.ToVector2Int(boardIndex);
              List<Vector2Int> validActions = s_validMovesPool.Get();

              var actionProvider = tile.Piece.ActionProvider;
              
              actionProvider.CalculateValidAttacks(boardPosition, board, validActions);
              actionProvider.CalculateValidMoves(boardPosition, board, validActions);

              for (int i = 0, n = validActions.Count; i < n; ++i)
              {
                 Vector2Int action = validActions[i];
                 Piece pieceToCapture = board.GetPieceAt(action);
                 Move move = new Move(tile.Piece, pieceToCapture, boardPosition, action);
                 
                 board.MakeMove(move);
                 int score = AlphaBetaMax(parameters, board, activeColour, rng, alpha, beta, remainingDepth - 1);
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

           for (int tileIndex = 0, n = board.board.Length; tileIndex < n; ++tileIndex)
           {
              Tile tile = board.board[tileIndex];
              if (!tile.IsValid || !tile.Piece.IsValid)
              {
                 continue;
              }

              Vector2Int position = board.ToVector2Int(tileIndex);
              int materialWorth = CalculateValue(
                 parameters,
                 position, 
                 tile.Piece, 
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
           int colourModifier = piece.PieceColour == activeColour ? 1 : -1;
           int chessPieceValue = piece.Value;
           ChessPieceType chessPieceType = piece.PieceType;
           int chessPieceValueOverride = parameters.GetValueOverrideForPiece(chessPieceType, rng);
           int positionImportance = boardLookup.GetTileImportance(position);
           int positionImportanceOverride = parameters.GetImportanceOverrideForPosition(position);
           
           int value = chessPieceValue + chessPieceValueOverride + positionImportance + positionImportanceOverride;
           return value * colourModifier;
        }
    }
}