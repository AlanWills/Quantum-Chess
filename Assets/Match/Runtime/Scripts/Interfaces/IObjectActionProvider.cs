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
            IReadOnlyCollection<Vector2Int> boardPositions,
            IList<Vector2Int> validMoves);
        void CalculateValidAttacks(
            Instance instance, 
            Vector2Int currentBoardPosition,
            IReadOnlyList<ChessBoardObjectInstance> otherObjects, 
            IList<Vector2Int> validMoves);
    }
}