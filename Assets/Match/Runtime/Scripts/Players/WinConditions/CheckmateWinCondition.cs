using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace QC.Match.Players
{
    public class CheckmateWinCondition : IWinCondition
    {
        private readonly PieceColour OppositionColour;
        private readonly List<Vector2Int> validActionsCache = new();
        
        public CheckmateWinCondition(PieceColour oppositionColour)
        {
            OppositionColour = oppositionColour;
        }
        
        public UniTask<bool> CalculateIfWinConditionIsMet(IWinCondition.Args args)
        {
            if (!args.Board.TryFindPiece(OppositionColour, PieceType.King, out var oppositionKing))
            {
                // The King straight up does not exist, game over!
                return UniTask.FromResult(true);
            }

            if (!oppositionKing.TryFindInterface<IActionProvider>(out var oppositionKingActionProvider))
            {
                // The King does not have an action provider... weird, game over!
                UnityEngine.Debug.LogAssertion($"The King piece for colour {OppositionColour} doesn't seem to have an {nameof(IActionProvider)} component...");
                return UniTask.FromResult(true);
            }

            if (!oppositionKing.TryFindInterface<IGrid2DTransform>(out var oppositionKingTransform))
            {
                // The King does not have a transform... weird, game over!
                UnityEngine.Debug.LogAssertion($"The King piece for colour {OppositionColour} doesn't seem to have an {nameof(IGrid2DTransform)} component...");
                return UniTask.FromResult(true);
            }

            IBoardActionMask playerBoardActionMask = args.BoardActionMaskLookup.GetBoardActionMaskForPlayer(args.PlayerIndex);
            Vector2Int oppositionKingPosition = oppositionKingTransform.GetPosition();

            if (!playerBoardActionMask.IsTileAttackable(oppositionKingPosition))
            {
                // King is not currently under attack so it can't be checkmate
                return UniTask.FromResult(false);
            }
            
            validActionsCache.Clear();
            oppositionKingActionProvider.CalculateValidMoves(
                oppositionKingPosition,
                args.Board,
                playerBoardActionMask,
                validActionsCache);

            if (validActionsCache.Count > 0)
            {
                // King has valid moves, so no checkmate
                return UniTask.FromResult(false);
            }

            validActionsCache.Clear();
            oppositionKingActionProvider.CalculateValidAttacks(
                oppositionKingPosition,
                args.Board,
                playerBoardActionMask,
                validActionsCache);

            if (validActionsCache.Count > 0)
            {
                // King has valid attacks, so no checkmate
                return UniTask.FromResult(false);
            }

            return UniTask.FromResult(true);
        }
    }
}