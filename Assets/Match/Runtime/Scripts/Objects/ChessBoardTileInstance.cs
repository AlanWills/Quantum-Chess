using System;
using Celeste.Components;
using QC.Match.Components;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace QC.Match.Objects
{
    public class ChessBoardTileInstance : ComponentContainerInstance<ChessBoardTileComponent>
    {
        #region Properties and Fields

        public int Guid => tile.Guid;
        public Vector2Int Position { get; }
        public TileBase Tile => tile.TileBase;

        [NonSerialized] private readonly ChessBoardTile tile;
        
        #endregion
        
        public ChessBoardTileInstance(ChessBoardTile tile, Vector2Int position)
        {
            this.tile = tile;
            
            Position = position;
            
            InitializeComponents(tile);
        }
    }
}