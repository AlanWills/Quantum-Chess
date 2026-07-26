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
        
        // Does this leak memory between domain reloads?
        private static readonly ObjectPool<List<Vector2Int>> s_validMovesPool = new(
           () => new List<Vector2Int>(k_defaultValidMovesAllocation),
           actionOnRelease: l => l.Clear(),
           defaultCapacity: k_defaultValidMovesPoolCapacity);
        private static TileBlit[] s_tiles;
        
        #endregion

        public static bool TryCalculate(
            ITurnStrategy.Args args,
            AlphaBetaParameters alphaBetaParameters,
            out ICommand<BoardCommandArgs> command)
        {
           if (alphaBetaParameters.Depth < 1)
           {
              UnityEngine.Debug.LogAssertion($"Failed to calculate best move as calculate depth was insufficient - please provide a value of at least 1.");
              command = null;
              return false;
           }
           
           using (AlphaBetaProfileMarkers.MainLoop.Auto())
           {
              ChessPieceColour activeColour = args.PlayerColour;
              Move bestMove = default;
              int bestScore = int.MinValue;
              int alpha = int.MinValue;
              int beta = int.MaxValue;
              
              RectInt boundingBox = args.Board.CalculateBoundingBox();
              if ( s_tiles == null || s_tiles.Length != boundingBox.size.x * boundingBox.size.y)
              {
                 s_tiles = new TileBlit[boundingBox.size.x * boundingBox.size.y];
              }
              
              BoardBlit boardBlit = new BoardBlit(boundingBox, s_tiles);
              boardBlit.SetTiles(args.Board.Tiles, alphaBetaParameters, args.RNG);
              boardBlit.SetObjects(args.Board.Objects, alphaBetaParameters, args.RNG);
              
              for (int boardIndex = 0, numBoardIndices = boardBlit.board.Length; boardIndex < numBoardIndices; ++boardIndex)
              {
                 TileBlit tileBlit = boardBlit.board[boardIndex];
              
                 if (!tileBlit.IsValid)
                 {
                    continue;
                 }
              
                 if (!tileBlit.Piece.IsValid)
                 {
                    continue;
                 }
              
                 if (tileBlit.Piece.PieceColour != activeColour)
                 {
                    continue;
                 }
                 
                 Piece piece = tileBlit.Piece;
                 Vector2Int position = boardBlit.ToVector2Int(boardIndex);

                 List<Vector2Int> validActions = s_validMovesPool.Get();
                 validActions.Clear();
                 
                 // TODO: May have to change these to support the faster AlphaBeta structs
                 IBoardActionMask opponentBoardActionMask = args.BoardActionMaskLookup.GetOpponentBoardActionMaskForPlayer(args.PlayerIndex);
                 tileBlit.Piece.ActionProvider.CalculateValidAttacks(position, args.Board, opponentBoardActionMask, validActions);
                 tileBlit.Piece.ActionProvider.CalculateValidMoves(position, args.Board, opponentBoardActionMask, validActions);

                 for (int i = 0, n = validActions.Count; i < n; ++i)
                 {
                    Vector2Int action = validActions[i];
                    Piece pieceToCapture = boardBlit.GetPieceAt(action);
                    Move move = new Move(piece, pieceToCapture, position, action);

                    boardBlit.MakeMove(move);
                    int score = AlphaBetaMin(alphaBetaParameters, boardBlit, activeColour, args.RNG, alpha, beta, alphaBetaParameters.Depth - 1);
                    boardBlit.UndoMove(move);

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

              var objectAtBestMove = args.Board.GetBoardObjectAt(bestMove.Destination);
              if (objectAtBestMove != null)
              {
                 // Our best move involves an attack
                 // We run a few basic checks first to ensure the algorithm hasn't done something weird
                 UnityEngine.Debug.Assert(args.Board.GetBoardObjectAt(bestMove.Position) != null,
                    $"Attempting to attack with a piece at {bestMove.Position}, but there is no object there!");
                 UnityEngine.Debug.Assert(objectAtBestMove.TryFindComponent<IChessPiece>(out var pieceToTake),
                    $"Failed to find a {nameof(IChessPiece)} interface on a piece that we have decided to take.");
                 UnityEngine.Debug.Assert(pieceToTake.GetPieceColour() != args.PlayerColour,
                    $"We're attempting to move on top of a piece at {bestMove.Destination} of the same colour.  Something has gone wrong with the AI here...");
                 command = new AttackWithChessBoardObjectFromToCommand(bestMove.Position, bestMove.Destination);
              }
              else
              {
                 // Our best move involves a move
                 // We run a few basic checks first to ensure the algorithm hasn't done something weird
                 UnityEngine.Debug.Assert(args.Board.GetBoardObjectAt(bestMove.Position) != null,
                    $"Attempting to move a piece at {bestMove.Position}, but there is no object there!");
                 command = new MoveChessBoardObjectFromToCommand(bestMove.Position, bestMove.Destination);
              }
           }

           return true;
        }

        private static int AlphaBetaMax(
           AlphaBetaParameters parameters,
           BoardBlit boardBlit,
           ChessPieceColour activeColour,
           IRNG rng,
           int alpha, 
           int beta,
           int remainingDepth)
        {
           if (remainingDepth == 0)
           {
              int currentBoardScore = EvaluateBoard(boardBlit, activeColour);
              return currentBoardScore;
           }

           int maxValue = int.MinValue;
           
           for (int boardIndex = 0, numBoardIndices = boardBlit.board.Length; boardIndex < numBoardIndices; ++boardIndex)
           {
              TileBlit tileBlit = boardBlit.board[boardIndex];
              
              if (!tileBlit.IsValid)
              {
                 continue;
              }
              
              if (!tileBlit.Piece.IsValid)
              {
                 continue;
              }
              
              if (tileBlit.Piece.PieceColour != activeColour)
              {
                 continue;
              }

              Vector2Int boardPosition = boardBlit.ToVector2Int(boardIndex);
              List<Vector2Int> validActions = s_validMovesPool.Get();
              
              var actionProvider = tileBlit.Piece.ActionProvider;
              
              // TODO: May have to change these to support the faster AlphaBeta structs
              actionProvider.CalculateValidAttacks(boardPosition, boardBlit, new EmptyBoardActionMask(), validActions);
              actionProvider.CalculateValidMoves(boardPosition, boardBlit, new EmptyBoardActionMask(), validActions);

              for (int i = 0, n = validActions.Count; i < n; ++i)
              {
                 Vector2Int action = validActions[i];
                 Piece pieceToCapture = boardBlit.GetPieceAt(action);
                 Move move = new Move(tileBlit.Piece, pieceToCapture, boardPosition, action);
                 
                 boardBlit.MakeMove(move);
                 int score = AlphaBetaMin(parameters, boardBlit, activeColour, rng, alpha, beta, remainingDepth - 1);
                 boardBlit.UndoMove(move);
                 
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
           BoardBlit boardBlit,
           ChessPieceColour activeColour,
           IRNG rng,
           int alpha, 
           int beta, 
           int remainingDepth)
        {
           if (remainingDepth == 0)
           {
              int currentBoardScore = EvaluateBoard(boardBlit, activeColour);
              return currentBoardScore;
           }
           
           int minValue = int.MaxValue;
           
           for (int boardIndex = 0, numBoardIndices = boardBlit.board.Length; boardIndex < numBoardIndices; ++boardIndex)
           {
              TileBlit tileBlit = boardBlit.board[boardIndex];
              
              if (!tileBlit.IsValid)
              {
                 continue;
              }
              
              if (!tileBlit.Piece.IsValid)
              {
                 continue;
              }
              
              if (tileBlit.Piece.PieceColour != activeColour)
              {
                 continue;
              }

              Vector2Int boardPosition = boardBlit.ToVector2Int(boardIndex);
              List<Vector2Int> validActions = s_validMovesPool.Get();

              var actionProvider = tileBlit.Piece.ActionProvider;
              
              // TODO: May have to change these to support the faster AlphaBeta structs
              actionProvider.CalculateValidAttacks(boardPosition, boardBlit, new EmptyBoardActionMask(), validActions);
              actionProvider.CalculateValidMoves(boardPosition, boardBlit, new EmptyBoardActionMask(), validActions);

              for (int i = 0, n = validActions.Count; i < n; ++i)
              {
                 Vector2Int action = validActions[i];
                 Piece pieceToCapture = boardBlit.GetPieceAt(action);
                 Move move = new Move(tileBlit.Piece, pieceToCapture, boardPosition, action);
                 
                 boardBlit.MakeMove(move);
                 int score = AlphaBetaMax(parameters, boardBlit, activeColour, rng, alpha, beta, remainingDepth - 1);
                 boardBlit.UndoMove(move);
                 
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

        private static int EvaluateBoard(BoardBlit boardBlit, ChessPieceColour activeColour)
        {
           int total = 0;

           for (int tileIndex = 0, n = boardBlit.board.Length; tileIndex < n; ++tileIndex)
           {
              // Interestingly, we don't account for the cost of NOT occupying a tile
              
              TileBlit tileBlit = boardBlit.board[tileIndex];
              if (!tileBlit.IsValid || !tileBlit.Piece.IsValid)
              {
                 continue;
              }
              
              int colourModifier = tileBlit.Piece.PieceColour == activeColour ? 1 : -1;
              int tileImportance = tileBlit.Importance;
              int pieceValue = tileBlit.Piece.Value;
              int totalValue = (tileImportance + pieceValue) * colourModifier;
              
              total += totalValue;
           }
           
           return total;
        }
    }
}