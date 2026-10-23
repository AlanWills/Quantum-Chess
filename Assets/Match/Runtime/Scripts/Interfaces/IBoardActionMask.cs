using UnityEngine;

namespace QC.Match
{
    public interface IBoardActionMask
    {
        bool IsTileAttackable(Vector2Int boardPosition);
    }

    public class EmptyBoardActionMask : IBoardActionMask
    {
        public bool IsTileAttackable(Vector2Int boardPosition)
        {
            return false;
        }
    }
}