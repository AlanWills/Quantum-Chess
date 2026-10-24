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
    public class BoardObjectActionView : MonoBehaviour
    {
        #region Properties and Fields

        [SerializeField] private MatchState matchState;
        [SerializeField] private Board board;
        [SerializeField] private Tilemap movementTilemap;
        [SerializeField] private Tilemap attackTilemap;
        [SerializeField] private TileBase validMovementTile;
        [SerializeField] private TileBase validAttackTile;

        [NonSerialized] private readonly List<Vector2Int> boardPositionCache = new();

        #endregion

        private void UpdateValidActionsForObject(BoardObjectInstance instance)
        {
            movementTilemap.ClearAllTiles();
            movementTilemap.CompressBounds();
            
            attackTilemap.ClearAllTiles();
            attackTilemap.CompressBounds();
            
            if (instance == null ||
                !instance.Grid2DTransform.IsValid ||
                !instance.ActionProvider.IsValid)
            {
                return;
            }

            Vector2Int boardPosition = instance.Grid2DTransform.GetPosition();
            CalculateValidMoves(boardPosition, instance.ActionProvider);
            CalculateValidAttacks(boardPosition, instance.ActionProvider);
        }

        private void CalculateValidMoves(Vector2Int boardPosition, InterfaceHandle<IActionProvider> actionProvider)
        {
            boardPositionCache.Clear();

            int activePlayerIndex = matchState.ActivePlayerIndex;
            IBoardActionMask opponentBoardActionMask = matchState.GetOpponentBoardActionMaskForPlayer(activePlayerIndex);
            actionProvider.CalculateValidMoves(boardPosition.x, boardPosition.y, board, opponentBoardActionMask, boardPositionCache);

            foreach (Vector2Int validMove in boardPositionCache)
            {
                if (!board.DoesBoardTileExist(validMove) || 
                    board.GetBoardObjectAt(validMove) != null)
                {
                    continue;
                }
                
                Vector2Int boardPositionInActionTilemapSpace = validMove + movementTilemap.origin.ToVector2Int();
                movementTilemap.SetTile(boardPositionInActionTilemapSpace.ToVector3Int(), validMovementTile);
            }
            
            boardPositionCache.Clear();
        }

        private void CalculateValidAttacks(Vector2Int boardPosition, InterfaceHandle<IActionProvider> actionProvider)
        {
            boardPositionCache.Clear();
            
            int activePlayerIndex = matchState.ActivePlayerIndex;
            IBoardActionMask opponentBoardActionMask = matchState.GetOpponentBoardActionMaskForPlayer(activePlayerIndex);
            actionProvider.CalculateValidAttacks(boardPosition.x, boardPosition.y, board, opponentBoardActionMask, boardPositionCache);

            foreach (Vector2Int validAttack in boardPositionCache)
            {
                if (!board.DoesBoardTileExist(validAttack) ||
                    board.GetBoardObjectAt(validAttack) == null)
                {
                    continue;
                }
                
                Vector2Int boardPositionInActionTilemapSpace = validAttack + attackTilemap.origin.ToVector2Int();
                attackTilemap.SetTile(boardPositionInActionTilemapSpace.ToVector3Int(), validAttackTile);
            }
            
            boardPositionCache.Clear();
        }

        private void CalculateValidThreats(Vector2Int boardPosition, InterfaceHandle<IActionProvider> actionProvider)
        {
            boardPositionCache.Clear();
            actionProvider.CalculateValidThreats(boardPosition.x, boardPosition.y, board, boardPositionCache);

            foreach (Vector2Int validAttack in boardPositionCache)
            {
                if (!board.DoesBoardTileExist(validAttack))
                {
                    continue;
                }
                
                Vector2Int boardPositionInActionTilemapSpace = validAttack + attackTilemap.origin.ToVector2Int();
                attackTilemap.SetTile(boardPositionInActionTilemapSpace.ToVector3Int(), validAttackTile);
            }
            
            boardPositionCache.Clear();
        }
        
        #region Callbacks

        public void OnChessBoardObjectSelectionChanged(BoardObjectSelectionChangedArgs args)
        {
            UpdateValidActionsForObject(args.NewSelectedObject);
        }
        
        #endregion
    }
}