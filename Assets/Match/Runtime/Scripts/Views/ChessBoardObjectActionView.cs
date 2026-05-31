using Celeste.Maths;
using QC.Match.Events;
using QC.Match.Objects;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace QC.Match.Views
{
    public class ChessBoardObjectActionView : MonoBehaviour
    {
        #region Properties and Fields
        
        [SerializeField] private Tilemap actionTilemap;
        [SerializeField] private TileBase validMovementTile;
        [SerializeField] private TileBase validAttackTile;

        #endregion

        private void UpdateValidActionsForObject(ChessBoardObjectInstance instance)
        {
            actionTilemap.ClearAllTiles();
            actionTilemap.CompressBounds();
            
            if (instance == null ||
                !instance.TryFindComponent<IGrid2DTransform>(out var grid2DTransform))
            {
                return;
            }
            
            Vector2Int cellPosition = grid2DTransform.iFace.GetPosition(grid2DTransform.instance).BoardToCellSpace();
            Vector2Int boardPositionInActionTilemapSpace = cellPosition + actionTilemap.origin.ToVector2Int();
            
            actionTilemap.SetTile(boardPositionInActionTilemapSpace.ToVector3Int() + new Vector3Int(1, 0, 0), validMovementTile);
            actionTilemap.SetTile(boardPositionInActionTilemapSpace.ToVector3Int() + new Vector3Int(-1, 0, 0), validMovementTile);
            actionTilemap.SetTile(boardPositionInActionTilemapSpace.ToVector3Int() + new Vector3Int(0, 1, 0), validMovementTile);
            actionTilemap.SetTile(boardPositionInActionTilemapSpace.ToVector3Int() + new Vector3Int(0, -1, 0), validMovementTile);
        }
        
        #region Callbacks

        public void OnChessBoardObjectSelectionChanged(ChessBoardObjectSelectionChangedArgs args)
        {
            UpdateValidActionsForObject(args.NewSelectedObject);
        }
        
        #endregion
    }
}