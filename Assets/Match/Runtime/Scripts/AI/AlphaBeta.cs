using System.Collections.Generic;
using Celeste.DataStructures;
using QC.Match.AI.Internals;
using QC.Match.Commands;
using QC.Match.Functors;
using UnityEngine;
using UnityEngine.Pool;

namespace QC.Match.AI
{
    public static class AlphaBeta
    {
       private struct ScoredMove
       {
          public Move Move;
          public float Score;
       }
       
       #region Properties and Fields
       
        private const int k_defaultValidMovesAllocation = 30;
        private const int k_defaultValidMovesPoolCapacity = 3000;
        private const int k_defaultScoredMovesAllocation = 100;

        private static readonly IBoardActionMask s_emptyBoardActionMask = new EmptyBoardActionMask();
        
        // Does this leak memory between domain reloads?
        private static readonly List<Vector2Int> s_validActionsCache = new(k_defaultValidMovesAllocation);
        private static readonly ObjectPool<List<Move>> s_movesToCheckPool = new(
           createFunc: () => new List<Move>(k_defaultScoredMovesAllocation),
           actionOnRelease: l => l.Clear(),
           defaultCapacity: k_defaultValidMovesPoolCapacity);
        private static readonly List<ScoredMove> s_scoredMovesCache = new(k_defaultScoredMovesAllocation);
        private static readonly List<TileBlit> s_tilesCache = new();
        
        #endregion

        public static bool TryCalculate(
            ITurnStrategy.Args args,
            AlphaBetaParameters alphaBetaParameters,
            out ICommand<BoardCommandArgs> command)
        {
           if (alphaBetaParameters.Depth < 1)
           {
              UnityEngine.Debug.LogAssertion("Failed to calculate best move as calculate depth was insufficient - please provide a value of at least 1.");
              command = null;
              return false;
           }
           
           using (AlphaBetaProfileMarkers.MainLoop.Auto())
           {
              // Bit of a hack, but can't find a more general solution right now
              // Our evaluation function is static and assumes white is positive and black is negative
              // As a result, white is looking for the highest scored move
              // And black is looking for the lowest scored move
              // We use this colour modifier to change the algo to always try and find the best move for the current player
              PieceColour activeColour = args.PlayerColour;
              int colorModifier = activeColour == PieceColour.White ? 1 : -1;

              // Use -int.MaxValue to prevent overflow when we invert signs in Negamax
              int bestScore = -int.MaxValue;
              int alpha = -int.MaxValue;
              int beta = int.MaxValue;
              
              RectInt boundingBox = args.Board.Bounds;
              s_tilesCache.Clear();
              s_scoredMovesCache.Clear();
              
              if (s_tilesCache.Count != boundingBox.size.x * boundingBox.size.y)
              {
                 s_tilesCache.Capacity = boundingBox.size.x * boundingBox.size.y;
                 s_tilesCache.Fill(default);
              }
              
              ColourFunctor matchActiveColour = ColourFunctor.Match(args.PlayerColour);
              BoardBlit boardBlit = new BoardBlit(boundingBox, s_tilesCache);
              boardBlit.SetTiles(args.Board.Tiles, alphaBetaParameters, args.RNG);
              boardBlit.SetObjects(args.Board.Objects, alphaBetaParameters, args.RNG);
              
              IBoardActionMask opponentBoardActionMask = args.BoardActionMaskLookup.GetOpponentBoardActionMaskForPlayer(args.PlayerIndex);
              List<Move> moves = s_movesToCheckPool.Get();
              GenerateMoves(boardBlit, matchActiveColour, opponentBoardActionMask, moves);

              for (int i = 0, n = moves.Count; i < n; ++i)
              {
                 Move move = moves[i];

                 boardBlit.MakeMove(move);
                 int score = -Negamax(boardBlit, matchActiveColour.Invert(), -beta, -alpha, alphaBetaParameters.Depth - 1, -colorModifier);
                 boardBlit.UndoMove(move);

                 s_scoredMovesCache.Add(new ScoredMove { Move = move, Score = score });

                 if (score > bestScore) bestScore = score;
                 if (score > alpha) alpha = score;

                 if (alpha >= beta) break;
              }

              s_movesToCheckPool.Release(moves);

              if (s_scoredMovesCache.Count == 0)
              {
                 UnityEngine.Debug.LogAssertion(
                    $"{nameof(AlphaBeta)} failed to find any valid moves (possibly Checkmate or Stalemate).");
                 command = null;
                 return false;
              }

              // Technically don't need to do this unless in the blunder branch (and instead keep track of the best move in the loop)
              // But I think it's fine
              s_scoredMovesCache.Sort((a, b) => a.Score.CompareTo(b.Score));

              Move bestMove = s_scoredMovesCache[^1].Move;

              if (args.RNG.FromRange01() <= alphaBetaParameters.BlunderChance)
              {
                 // We always ignore the best move when doing a blunder otherwise it wouldn't be a blunder!
                 int newMoveIndex = args.RNG.FromRangeInclusive(0, s_scoredMovesCache.Count - 1);
                 bestMove = s_scoredMovesCache[newMoveIndex].Move;
              }
              
              UnityEngine.Debug.Assert(bestMove.PieceToMove.PieceColour == activeColour,
                 $"Attempting to perform a move with a piece that is not the active colour: '{activeColour}'!");

              var objectAtBestMove = args.Board.GetBoardObjectAt(bestMove.DestinationX, bestMove.DestinationY);
              if (objectAtBestMove != null)
              {
                 // Our best move involves an attack
                 // We run a few basic checks first to ensure the algorithm hasn't done something weird
                 UnityEngine.Debug.Assert(args.Board.GetBoardObjectAt(bestMove.PositionX, bestMove.PositionY) != null,
                    $"Attempting to attack with a piece at ({bestMove.PositionX}, {bestMove.PositionY}), but there is no object there!");
                 UnityEngine.Debug.Assert(objectAtBestMove.Piece.IsValid,
                    $"Failed to find a {nameof(IPiece)} interface on a piece that we have decided to take.");
                 UnityEngine.Debug.Assert(objectAtBestMove.Piece.GetPieceColour() != args.PlayerColour,
                    $"We're attempting to move on top of a piece at ({bestMove.DestinationX}, {bestMove.DestinationY}) of the same colour.  Something has gone wrong with the AI here...");
                 command = new AttackWithBoardObjectFromToCommand(new Vector2Int(bestMove.PositionX, bestMove.PositionY), new Vector2Int(bestMove.DestinationX, bestMove.DestinationY));
              }
              else
              {
                 // Our best move involves a move
                 // We run a few basic checks first to ensure the algorithm hasn't done something weird
                 UnityEngine.Debug.Assert(args.Board.GetBoardObjectAt(bestMove.PositionX, bestMove.PositionY) != null,
                    $"Attempting to move a piece at ({bestMove.PositionX}, {bestMove.PositionY}), but there is no object there!");
                 command = new MoveBoardObjectFromToCommand(new Vector2Int(bestMove.PositionX, bestMove.PositionY), new Vector2Int(bestMove.DestinationX, bestMove.DestinationY));
              }
           }

           return true;
        }

        private static int Negamax(
           BoardBlit boardBlit,
           ColourFunctor colourFunctor,
           int alpha,
           int beta,
           int remainingDepth,
           int colorModifier)
        {
           if (remainingDepth == 0)
           {
              return EvaluateBoard(boardBlit) * colorModifier;
           }

           int maxValue = -int.MaxValue;

           List<Move> moves = s_movesToCheckPool.Get();
    
           // Generate ALL moves for the current player in one go
           GenerateMoves(boardBlit, colourFunctor, s_emptyBoardActionMask, moves);

           for (int i = 0, n = moves.Count; i < n; ++i)
           {
              Move move = moves[i];

              boardBlit.MakeMove(move);
              int score = -Negamax(boardBlit, colourFunctor.Invert(), -beta, -alpha, remainingDepth - 1, -colorModifier);
              boardBlit.UndoMove(move);

              if (score > maxValue) maxValue = score;
              if (score > alpha) alpha = score;

              // Pruning!
              if (alpha >= beta) break;
           }

           s_movesToCheckPool.Release(moves);

           return maxValue;
        }
        
        private static void GenerateMoves(
           BoardBlit boardBlit,
           ColourFunctor colourFunctor,
           IBoardActionMask actionMask,
           List<Move> outMoves)
        {
           for (int boardIndex = 0, numBoardIndices = boardBlit.board.Count; boardIndex < numBoardIndices; ++boardIndex)
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
              
              if (!colourFunctor.IsAMatch(tileBlit.Piece.PieceColour))
              {
                 continue;
              }
              
              int boardPositionX = boardBlit.ToX(boardIndex);
              int boardPositionY = boardBlit.ToY(boardIndex);
              s_validActionsCache.Clear();
              
              var actionProvider = tileBlit.Piece.ActionProvider;
              actionProvider.CalculateValidAttacks(boardPositionX, boardPositionY, boardBlit, actionMask, s_validActionsCache);
              actionProvider.CalculateValidMoves(boardPositionX, boardPositionY, boardBlit, actionMask, s_validActionsCache);

              for (int i = 0, n = s_validActionsCache.Count; i < n; ++i)
              {
                 Vector2Int action = s_validActionsCache[i];
#if ULTRA_DEBUG_CHECKS
                 UnityEngine.Debug.Assert(boardBlit.IsPositionInBoard(action), $"Generated an invalid action ({action}) for Piece: {tileBlit.Piece.PieceColour} - {tileBlit.Piece.PieceType} - {boardPosition}.");
#endif
                 Piece pieceToCapture = boardBlit.GetPieceAt(action.x, action.y);
#if ULTRA_DEBUG_CHECKS
                 UnityEngine.Debug.Assert(outMoves.Capacity > outMoves.Count, $"Reallocation!  Capacity: {outMoves.Capacity}, Count: {outMoves.Count}.");
#endif
                 outMoves.Add(new Move(tileBlit.Piece, pieceToCapture, boardPositionX, boardPositionY, action.x, action.y));
              }
           }
           
           SortNoGC(outMoves);
        }

        private static void SortNoGC(IList<Move> list)
        {
           int count = list.Count;
           for (int i = 1; i < count; i++)
           {
              Move key = list[i];
              int j = i - 1;

              // Shift elements until the correct position for 'key' is found
              while (j >= 0 && list[j].CompareTo(in key) > 0)
              {
                 list[j + 1] = list[j];
                 j--;
              }
              list[j + 1] = key;
           }
        }
        
        private static int EvaluateBoard(BoardBlit boardBlit)
        {
           int total = 0;

           for (int tileIndex = 0, n = boardBlit.board.Count; tileIndex < n; ++tileIndex)
           {
              // Interestingly, we don't account for the cost of NOT occupying a tile
              
              TileBlit tileBlit = boardBlit.board[tileIndex];
              if (!tileBlit.IsValid || !tileBlit.Piece.IsValid)
              {
                 continue;
              }

              // Bit of a hack, but can't find a more general solution right now
              // Our evaluation function is static and assumes white is positive and black is negative
              // As a result, white is looking for the highest scored move
              // And black is looking for the lowest scored move
              // We account for this further up in Negamax
              int colourModifier = tileBlit.Piece.PieceColour == PieceColour.White ? 1 
                 : tileBlit.Piece.PieceColour == PieceColour.Black ? -1 : 0;
              int tileImportance = tileBlit.Importance;
              int pieceValue = tileBlit.Piece.Value;
              int totalValue = (tileImportance + pieceValue) * colourModifier;
              
              total += totalValue;
           }
           
           return total;
        }
    }
}