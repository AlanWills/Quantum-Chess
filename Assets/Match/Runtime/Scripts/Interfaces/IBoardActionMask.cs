using UnityEngine;

namespace QC.Match
{
    public interface IBoardActionMask
    {
        bool IsTileAttackable(int x, int y);
    }

    public class EmptyBoardActionMask : IBoardActionMask
    {
        public bool IsTileAttackable(int x, int y)
        {
            return false;
        }
    }
}