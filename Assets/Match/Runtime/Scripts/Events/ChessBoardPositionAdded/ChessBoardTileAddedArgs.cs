using UnityEngine;
using UnityEngine.Tilemaps;

namespace QC.Match.Events
{
    public readonly struct ChessBoardTileAddedArgs
    {
        public readonly Vector2Int BoardPosition;
        public readonly TileBase Tile;

        public ChessBoardTileAddedArgs(Vector2Int boardPosition, TileBase tile)
        {
            BoardPosition = boardPosition;
            Tile = tile;
        }
    }
}