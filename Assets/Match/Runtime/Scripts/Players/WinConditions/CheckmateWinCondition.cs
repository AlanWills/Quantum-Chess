using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace QC.Match.Players
{
    public class CheckmateWinCondition : IWinCondition
    {
        private readonly ChessPieceColour OppositionColour;
        private readonly List<Vector2Int> validActionsCache = new();
        
        public CheckmateWinCondition(ChessPieceColour oppositionColour)
        {
            OppositionColour = oppositionColour;
        }
        
        public UniTask<bool> CalculateIfWinConditionIsMet(IWinCondition.Args args)
        {
            if (!args.Board.TryFindChessPiece(OppositionColour, ChessPieceType.King, out var oppositionKing))
            {
                // The King straight up does not exist, game over!
                return UniTask.FromResult(true);
            }

            if (!oppositionKing.TryFindComponent<IActionProvider>(out var oppositionKingActionProvider))
            {
                // The King does not have an action provider... weird, game over!
                UnityEngine.Debug.LogAssertion($"The King piece for colour {OppositionColour} doesn't seem to have an {nameof(IActionProvider)} component...");
                return UniTask.FromResult(true);
            }

            if (!oppositionKing.TryFindComponent<IGrid2DTransform>(out var oppositionKingTransform))
            {
                // The King does not have a transform... weird, game over!
                UnityEngine.Debug.LogAssertion($"The King piece for colour {OppositionColour} doesn't seem to have an {nameof(IGrid2DTransform)} component...");
                return UniTask.FromResult(true);
            }

            validActionsCache.Clear();
            oppositionKingActionProvider.CalculateValidMoves(
                oppositionKingTransform.GetPosition(),
                args.Board,
                args.PlayerActionMaskLookup,
                validActionsCache);

            if (validActionsCache.Count > 0)
            {
                // King has valid moves, so no checkmate
                return UniTask.FromResult(false);
            }

            validActionsCache.Clear();
            oppositionKingActionProvider.CalculateValidAttacks(
                oppositionKingTransform.GetPosition(),
                args.Board,
                args.PlayerActionMaskLookup,
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