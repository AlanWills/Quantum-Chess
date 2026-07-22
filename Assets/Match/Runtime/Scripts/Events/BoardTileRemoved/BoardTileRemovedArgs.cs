using UnityEngine;

namespace QC.Match.Events
{
    public readonly struct BoardTileRemovedArgs
    {
        public readonly Vector2Int BoardPosition;

        public BoardTileRemovedArgs(Vector2Int boardPosition)
        {
            BoardPosition = boardPosition;
        }
    }
}