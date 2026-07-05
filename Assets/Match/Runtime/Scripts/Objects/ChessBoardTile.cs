using Celeste.Components;
using Celeste.Objects;
using Celeste.Tools;
using QC.Match.Components;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace QC.Match.Objects
{
    [CreateAssetMenu(fileName = nameof(ChessBoardTile), menuName = "QC/Match/Tiles/Chess Board Tile")]
    public class ChessBoardTile : ComponentContainerUsingSubAssets<ChessBoardTileComponent>, IIntGuid
    {
        #region Properties and Fields

        public int Guid
        {
            get => guid;
            set
            {
                if (guid != value)
                {
                    guid = value;
                    EditorOnly.SetDirty(this);
                }
            }
        }

        public TileBase TileBase => tileBase;
        
        [SerializeField] private int guid;
        [SerializeField] private TileBase tileBase;

        #endregion
    }
}