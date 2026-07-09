using System;
using System.Collections.Generic;
using Celeste.Components;
using Celeste.Core.Interfaces;
using QC.Match.AI.Internals;
using QC.Match.Commands;
using QC.Match.Objects;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Windows;

namespace QC.Match.AI
{
    public static class AlphaBeta
    {
       #region Properties and Fields
       
        private const int k_defaultValidMovesAllocation = 20;
        private const int k_defaultValidMovesPoolCapacity = 1000;
        
        private static ObjectPool<List<Vector2Int>> s_validMovesPool = new(
           () => new List<Vector2Int>(k_defaultValidMovesAllocation),
           actionOnRelease: l => l.Clear(),
           defaultCapacity: k_defaultValidMovesPoolCapacity);
        private static Tile[] s_tiles;
        
        #endregion
        
        public static bool TryCalculate(
            ITurnStrategy.Args args,
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
              
              Board board = new Board(boundingBox, s_tiles);
              board.SetTiles(args.ChessBoard.Tiles, alphaBetaParameters);
              board.SetObjects(args.ChessBoard.Objects, alphaBetaParameters, args.RNG);
              
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

                 
                 Piece piece = tile.Piece;
                 Vector2Int position = board.ToVector2Int(boardIndex);

                 List<Vector2Int> validActions = s_validMovesPool.Get();
                 tile.Piece.ActionProvider.CalculateValidAttacks(position, args.ChessBoard, validActions);
                 tile.Piece.ActionProvider.CalculateValidMoves(position, args.ChessBoard, validActions);

                 for (int i = 0, n = validActions.Count; i < n; ++i)
                 {
                    Vector2Int action = validActions[i];
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
              int currentBoardScore = EvaluateBoard(board, activeColour);
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
              
              // TODO: May have to change these to support the faster AlphaBeta structs
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
              int currentBoardScore = EvaluateBoard(board, activeColour);
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

        private static int EvaluateBoard(Board board, ChessPieceColour activeColour)
        {
           int total = 0;

           for (int tileIndex = 0, n = board.board.Length; tileIndex < n; ++tileIndex)
           {
              // Interestingly, we don't account for the cost of NOT occupying a tile
              
              Tile tile = board.board[tileIndex];
              if (!tile.IsValid || !tile.Piece.IsValid)
              {
                 continue;
              }
              
              int colourModifier = tile.Piece.PieceColour == activeColour ? 1 : -1;
              int tileImportance = tile.Importance;
              int pieceValue = tile.Piece.Value;
              int totalValue = (tileImportance + pieceValue) * colourModifier;
              
              total += totalValue;
           }
           
           return total;
        }
    }
}