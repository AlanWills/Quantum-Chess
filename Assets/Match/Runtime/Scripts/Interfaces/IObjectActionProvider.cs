using System.Collections.Generic;
using Celeste.Components;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match
{
    public interface IObjectActionProvider
    {
        void CalculateValidMoves(
            Instance instance, 
            Vector2Int currentBoardPosition,
            IChessBoardLookup chessBoardLookup, 
            IList<Vector2Int> validMoves);
        void CalculateValidAttacks(
            Instance instance, 
            Vector2Int currentBoardPosition,
            IChessBoardLookup chessBoardLookup, 
            IList<Vector2Int> validAttacks);
    }
}