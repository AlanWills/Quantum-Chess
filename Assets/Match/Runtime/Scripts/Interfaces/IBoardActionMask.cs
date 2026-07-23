using UnityEngine;

namespace QC.Match
{
    public interface IBoardActionMask
    {
        bool IsTileAttackable(Vector2Int boardPosition);
    }

    public struct NoAttacksBoardActionMask : IBoardActionMask
    {
        public bool IsTileAttackable(Vector2Int boardPosition)
        {
            return false;
        }
    }
}