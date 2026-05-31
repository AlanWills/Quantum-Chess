using UnityEngine;

namespace QC.Match
{
    public static class ChessBoardMathsExtensions
    {
        // Board space is 1-indexed
        public static Vector2Int CellToBoardSpace(this Vector3Int v)
        {
            return new Vector2Int(v.x + 1, v.y + 1);
        }
        
        public static Vector2Int BoardToCellSpace(this Vector2Int v)
        {
            return new Vector2Int(v.x - 1, v.y - 1);
        }
    }
}