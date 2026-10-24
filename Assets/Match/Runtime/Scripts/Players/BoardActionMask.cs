using System.Runtime.CompilerServices;
using UnityEngine;

namespace QC.Match.Players
{
    public class BoardActionMask : IBoardActionMask
    {
        private readonly TileActionMask[] board;
        private readonly int width;
        private readonly int height;
        private readonly int xMin;
        private readonly int yMin;

        public BoardActionMask(RectInt boardDimensions)
        {
            width = boardDimensions.size.x;
            height = boardDimensions.size.y;
            xMin = boardDimensions.xMin;
            yMin = boardDimensions.yMin;
            board = new TileActionMask[width * height];
        }

        public bool MatchesBoundingBox(RectInt boundingBox)
        {
            return boundingBox.xMin == xMin && 
                   boundingBox.yMin == yMin && 
                   boundingBox.size.x == width &&
                   boundingBox.size.y == height;
        }

        public void ResetAllTiles()
        {
            for (int i = 0, n = board.Length; i < n; ++i)
            {
                board[i] = new TileActionMask(false);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetTileAttackable(int positionX, int positionY)
        {
            int positionIndex = ToIndex(positionX, positionY);
#if ULTRA_DEBUG_CHECKS
            UnityEngine.Debug.Assert(0 <= positionIndex && positionIndex < board.Length, $"Invalid position {position} entered into {nameof(SetTileAttackable)}.  Board array size is: {board.Length}.");
#endif
            board[positionIndex] = new TileActionMask(true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsTileAttackable(int positionX, int positionY)
        {
            int positionIndex = ToIndex(positionX, positionY);
            return 0 <= positionIndex && positionIndex < board.Length && board[positionIndex].CanPieceAttack;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ToIndex(int positionX, int positionY)
        {
            int index = (positionX - xMin) + (positionY - yMin) * width;
            return index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vector2Int ToVector2Int(int boardIndex)
        {
            return new Vector2Int((boardIndex % width) + xMin, (boardIndex / width) + yMin);
        }
    }
}