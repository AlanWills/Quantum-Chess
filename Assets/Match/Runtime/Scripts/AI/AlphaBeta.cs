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
       
        private const int k_defaultValidMovesAllocation = 20;
        private const int k_defaultScoredMovesAllocation = 100;
        private const int k_defaultValidMovesPoolCapacity = 1000;
        
        private static readonly MoveOrderingComparer s_moveComparer = new();
        
        // Does this leak memory between domain reloads?
        private static readonly ObjectPool<List<Vector2Int>> s_validMovesPool = new(
           () => new List<Vector2Int>(k_defaultValidMovesAllocation),
           actionOnRelease: l => l.Clear(),
           defaultCapacity: k_defaultValidMovesPoolCapacity);
        private static readonly ObjectPool<List<Move>> s_movesToCheckPool = new(
           () => new List<Move>(k_defaultScoredMovesAllocation),
           actionOnRelease: l => l.Clear(),
           defaultCapacity: k_defaultValidMovesPoolCapacity);
        private static readonly ObjectPool<List<ScoredMove>> s_scoredMovesPool = new(
           () => new List<ScoredMove>(k_defaultScoredMovesAllocation),
           actionOnRelease: l => l.Clear(),
           defaultCapacity: 1);
        private static readonly ObjectPool<List<TileBlit>> s_tilesPool = new(
           () => new List<TileBlit>(),
           actionOnRelease: l => l.Clear(),
           defaultCapacity: 1);
        
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
              // Bit of a hack, but can't find a more general solution right now
              // Our evaluation function is static and assumes white is positive and black is negative
              // As a result, white is looking for the highest scored move
              // And black is looking for the lowest scored move
              // We use this colour modifier to change the algo to always try and find the best move for the current player
              ChessPieceColour activeColour = args.PlayerColour;
              int colorModifier = activeColour == ChessPieceColour.White ? 1 : -1;

              // Use -int.MaxValue to prevent overflow when we invert signs in Negamax
              int bestScore = -int.MaxValue;
              int alpha = -int.MaxValue;
              int beta = int.MaxValue;
              
              RectInt boundingBox = args.Board.CalculateBoundingBox();
              List<TileBlit> tiles = s_tilesPool.Get();
              
              if (tiles.Count != boundingBox.size.x * boundingBox.size.y)
              {
                 tiles.Capacity = boundingBox.size.x * boundingBox.size.y;
                 tiles.Fill(default);
              }
              
              IColourFunctor matchActiveColour = new MatchColourFunctor(args.PlayerColour);
              BoardBlit boardBlit = new BoardBlit(boundingBox, tiles);
              boardBlit.SetTiles(args.Board.Tiles, alphaBetaParameters, args.RNG);
              boardBlit.SetObjects(args.Board.Objects, alphaBetaParameters, args.RNG);
              
              IBoardActionMask opponentBoardActionMask = args.BoardActionMaskLookup.GetOpponentBoardActionMaskForPlayer(args.PlayerIndex);
              List<ScoredMove> scoredMoves = s_scoredMovesPool.Get();
              List<Move> moves = s_movesToCheckPool.Get();
              GenerateMoves(boardBlit, matchActiveColour, opponentBoardActionMask, moves);

              for (int i = 0, n = moves.Count; i < n; ++i)
              {
                 Move move = moves[i];

                 boardBlit.MakeMove(move);
                 int score = -Negamax(boardBlit, matchActiveColour.Invert(), -beta, -alpha, alphaBetaParameters.Depth - 1, -colorModifier);
                 boardBlit.UndoMove(move);

                 scoredMoves.Add(new ScoredMove { Move = move, Score = score });

                 if (score > bestScore) bestScore = score;
                 if (score > alpha) alpha = score;

                 if (alpha >= beta) break;
              }

              s_movesToCheckPool.Release(moves);
              s_tilesPool.Release(tiles);

              if (scoredMoves.Count == 0)
              {
                 s_scoredMovesPool.Release(scoredMoves);
                 
                 UnityEngine.Debug.LogAssertion(
                    $"{nameof(AlphaBeta)} failed to find any valid moves (possibly Checkmate or Stalemate).");
                 command = null;
                 return false;
              }

              // Technically don't need to do this unless in the blunder branch (and instead keep track of the best move in the loop)
              // But I think it's fine
              scoredMoves.Sort((a, b) => a.Score.CompareTo(b.Score));

              Move bestMove = scoredMoves[^1].Move;

              if (args.RNG.FromRange01() <= alphaBetaParameters.BlunderChance)
              {
                 // We always ignore the best move when doing a blunder otherwise it wouldn't be a blunder!
                 int newMoveIndex = args.RNG.FromRangeInclusive(0, scoredMoves.Count - 1);
                 bestMove = scoredMoves[newMoveIndex].Move;
              }
              
              s_scoredMovesPool.Release(scoredMoves);
              
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

        private static int Negamax(
           BoardBlit boardBlit,
           IColourFunctor colourFunctor,
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
           GenerateMoves(boardBlit, colourFunctor, new EmptyBoardActionMask(), moves);

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
           IColourFunctor colourFunctor,
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

              Vector2Int boardPosition = boardBlit.ToVector2Int(boardIndex);
              List<Vector2Int> validActions = s_validMovesPool.Get();

              var actionProvider = tileBlit.Piece.ActionProvider;
              actionProvider.CalculateValidAttacks(boardPosition, boardBlit, actionMask, validActions);
              actionProvider.CalculateValidMoves(boardPosition, boardBlit, actionMask, validActions);

              for (int i = 0, n = validActions.Count; i < n; ++i)
              {
                 Vector2Int action = validActions[i];
                 Piece pieceToCapture = boardBlit.GetPieceAt(action);
                 outMoves.Add(new Move(tileBlit.Piece, pieceToCapture, boardPosition, action));
                 
                 UnityEngine.Debug.Assert(tileBlit.Piece.Instance.FindInterface<IGrid2DTransform>().GetPosition() == boardPosition,
                    $"Piece Found At {tileBlit.Piece.Instance.FindInterface<IGrid2DTransform>().GetPosition()}, but Move created from {boardPosition}.");
              }

              s_validMovesPool.Release(validActions);
           }
           
           outMoves.Sort(s_moveComparer);
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
              int colourModifier = tileBlit.Piece.PieceColour == ChessPieceColour.White ? 1 
                 : tileBlit.Piece.PieceColour == ChessPieceColour.Black ? -1 : 0;
              int tileImportance = tileBlit.Importance;
              int pieceValue = tileBlit.Piece.Value;
              int totalValue = (tileImportance + pieceValue) * colourModifier;
              
              total += totalValue;
           }
           
           return total;
        }
    }
}