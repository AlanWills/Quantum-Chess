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
            List<ChessBoardObjectInstance> piecesToConsider,
            ITurnStrategy.Args args,
            out ICommand<ChessBoardCommandArgs> command)
        {
            List<Vector2Int> validAttacks = new();
            List<Vector2Int> validMoves = new();
            
            // Shuffle them up
            piecesToConsider.Shuffle();

            for (int i = 0; i < piecesToConsider.Count; --i)
            {
                ChessBoardObjectInstance chessPiece = piecesToConsider[i];

                if (chessPiece.TryFindComponent<IGrid2DTransform>(out var transform) &&
                    chessPiece.TryFindComponent<IActionProvider>(out var actionProvider))
                {
                    validAttacks.Clear();
                    validMoves.Clear();
                    
                    actionProvider.iFace.CalculateValidAttacks(
                        actionProvider.instance,
                        transform.iFace.GetPosition(transform.instance), 
                        args.ChessBoard, 
                        validAttacks);
                    
                    actionProvider.iFace.CalculateValidMoves(
                        actionProvider.instance,
                        transform.iFace.GetPosition(transform.instance), 
                        args.ChessBoard, 
                        validMoves);
                    
                    int totalActions = validAttacks.Count + validMoves.Count;
                    if (totalActions > 0)
                    {
                        int randomActionIndex = Random.Range(0, validAttacks.Count + validMoves.Count);

                        if (randomActionIndex < validAttacks.Count)
                        {
                            command = new AttackWithChessBoardObjectFromToCommand(
                                transform.iFace.GetPosition(transform.instance),
                                validAttacks[randomActionIndex]);
                            return true;
                        }
                        else
                        {
                            command = new MoveChessBoardObjectToCommand(
                                chessPiece,
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