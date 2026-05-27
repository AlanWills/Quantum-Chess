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
            if (instance == null ||
                !instance.TryFindComponent<IGrid2DTransform>(out var grid2DTransform))
            {
                actionTilemap.ClearAllTiles();
                return;
            }

            // Position in world space
            Vector2Int position = grid2DTransform.iFace.GetPosition(grid2DTransform.instance);
            
            // Board Positions are 1-indexed
            Vector2Int boardPosition = position - new Vector2Int(1, 1);
            Vector2Int boardPositionInActionTilemapSpace = boardPosition + actionTilemap.origin.ToVector2Int();
            
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