using UnityEngine;

namespace QC.Match.Players
{
    public class BoardActionMask
    {
        private readonly TileActionMask[] board;
        private readonly int width;
        private readonly int xMin;
        private readonly int yMin;

        public BoardActionMask(RectInt boardDimensions, TileActionMask[] tileActionMaskArray)
        {
            width = boardDimensions.size.x;
            xMin = boardDimensions.xMin;
            yMin = boardDimensions.yMin;
            board = tileActionMaskArray;
        }

        public void ResetAllTiles()
        {
            for (int i = 0, n = board.Length; i < n; ++i)
            {
                TileActionMask existingMask = board[i];
                board[i] = new TileActionMask(false);
            }
        }

        public void SetTileAttackable(Vector2Int position)
        {
            int positionIndex = ToIndex(position);
            TileActionMask existingTileMask = board[positionIndex];
            board[positionIndex] = new TileActionMask(true);
        }

        public int ToIndex(Vector2Int position)
        {
            int index = (position.x - xMin) + (position.y - yMin) * width;
            return index;
        }

        public Vector2Int ToVector2Int(int boardIndex)
        {
            return new Vector2Int((boardIndex % width) + xMin, (boardIndex / width) + yMin);
        }
    }
}