using System;
using System.Collections.Generic;
using Celeste.Components;
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

        [SerializeField] private ChessBoard chessBoard;
        [SerializeField] private Tilemap actionTilemap;
        [SerializeField] private TileBase validMovementTile;
        [SerializeField] private TileBase validAttackTile;

        [NonSerialized] private readonly List<Vector2Int> boardPositionCache = new();

        #endregion

        private void UpdateValidActionsForObject(ChessBoardObjectInstance instance)
        {
            actionTilemap.ClearAllTiles();
            actionTilemap.CompressBounds();
            
            if (instance == null ||
                !instance.TryFindComponent<IGrid2DTransform>(out var grid2DTransform) ||
                !instance.TryFindComponent<IObjectActionProvider>(out var actionProvider))
            {
                return;
            }

            Vector2Int boardPosition = grid2DTransform.iFace.GetPosition(grid2DTransform.instance);
            CalculateValidMoves(boardPosition, actionProvider);
            CalculateValidAttacks(boardPosition, actionProvider);
        }

        private void CalculateValidMoves(Vector2Int boardPosition, InterfaceHandle<IObjectActionProvider> actionProvider)
        {
            boardPositionCache.Clear();
            actionProvider.iFace.CalculateValidMoves(actionProvider.instance, boardPosition, chessBoard, boardPositionCache);

            foreach (Vector2Int validMove in boardPositionCache)
            {
                if (!chessBoard.DoesChessBoardTileExist(validMove) || 
                    chessBoard.GetChessBoardObjectAt(validMove) != null)
                {
                    continue;
                }
                
                Vector2Int boardPositionInActionTilemapSpace = validMove + actionTilemap.origin.ToVector2Int();
                actionTilemap.SetTile(boardPositionInActionTilemapSpace.ToVector3Int(), validMovementTile);
            }
            
            boardPositionCache.Clear();
        }

        private void CalculateValidAttacks(Vector2Int boardPosition, InterfaceHandle<IObjectActionProvider> actionProvider)
        {
            boardPositionCache.Clear();
            actionProvider.iFace.CalculateValidAttacks(actionProvider.instance, boardPosition, chessBoard, boardPositionCache);

            foreach (Vector2Int validAttack in boardPositionCache)
            {
                if (!chessBoard.DoesChessBoardTileExist(validAttack) ||
                    chessBoard.GetChessBoardObjectAt(validAttack) == null)
                {
                    continue;
                }
                
                Vector2Int boardPositionInActionTilemapSpace = validAttack + actionTilemap.origin.ToVector2Int();
                actionTilemap.SetTile(boardPositionInActionTilemapSpace.ToVector3Int(), validAttackTile);
            }
            
            boardPositionCache.Clear();
        }
        
        #region Callbacks

        public void OnChessBoardObjectSelectionChanged(ChessBoardObjectSelectionChangedArgs args)
        {
            UpdateValidActionsForObject(args.NewSelectedObject);
        }
        
        #endregion
    }
}