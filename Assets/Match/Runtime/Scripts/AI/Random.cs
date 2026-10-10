using System.Collections.Generic;
using Celeste.DataStructures;
using QC.Match.Commands;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.AI
{
    public static class RandomAction
    {
        public static bool TryCalculate(
            ITurnStrategy.Args args,
            List<BoardObjectInstance> piecesToConsider,
            out ICommand<BoardCommandArgs> command)
        {
            List<Vector2Int> validAttacks = new();
            List<Vector2Int> validMoves = new();
            
            // Shuffle them up
            piecesToConsider.Shuffle();

            for (int i = 0; i < piecesToConsider.Count; --i)
            {
                BoardObjectInstance piece = piecesToConsider[i];

                if (piece.TryFindInterface<IGrid2DTransform>(out var transform) &&
                    piece.TryFindInterface<IActionProvider>(out var actionProvider))
                {
                    validAttacks.Clear();
                    validMoves.Clear();

                    IBoardActionMask opponentBoardActionMask = args.BoardActionMaskLookup.GetOpponentBoardActionMaskForPlayer(args.PlayerIndex);
                    actionProvider.CalculateValidAttacks(
                        transform.GetPosition(), 
                        args.Board,
                        opponentBoardActionMask,
                        validAttacks);
                    
                    actionProvider.CalculateValidMoves(
                        transform.GetPosition(), 
                        args.Board,
                        opponentBoardActionMask,
                        validMoves);
                    
                    int totalActions = validAttacks.Count + validMoves.Count;
                    if (totalActions > 0)
                    {
                        int randomActionIndex = UnityEngine.Random.Range(0, validAttacks.Count + validMoves.Count);

                        if (randomActionIndex < validAttacks.Count)
                        {
                            command = new AttackWithBoardObjectFromToCommand(
                                transform.GetPosition(),
                                validAttacks[randomActionIndex]);
                            return true;
                        }
                        else
                        {
                            command = new MoveBoardObjectToCommand(
                                piece,
                                validMoves[randomActionIndex - validAttacks.Count]);
                            return true;
                        }
                    }
                }
            }

            command = default;
            return false;
        }
    }
}