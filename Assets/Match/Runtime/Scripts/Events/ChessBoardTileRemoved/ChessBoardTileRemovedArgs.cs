using UnityEngine;

namespace QC.Match.Events
{
    public readonly struct ChessBoardTileRemovedArgs
    {
        public readonly Vector2Int BoardPosition;

        public ChessBoardTileRemovedArgs(Vector2Int boardPosition)
        {
            BoardPosition = boardPosition;
        }
    }
}