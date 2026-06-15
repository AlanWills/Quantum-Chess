using System;
using System.Collections.Generic;
using Celeste.Components;
using Celeste.Input;
using Celeste.Maths;
using QC.Match.Commands;
using QC.Match.Events;
using QC.Match.Objects;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace QC.Match.Views
{
    public class ChessBoardView : MonoBehaviour
    {
        #region Properties and Fields
        
        [SerializeField] private ChessBoardObjectView chessBoardObjectView;
        [SerializeField] private ChessBoardCommandExecutor chessBoardCommandExecutor;
        [SerializeField] private Transform piecesViewRoot;
        [SerializeField] private Tilemap chessBoardTilemap;
        [SerializeField] private Collider2D hitBoxCollider;

        [Header("Events")] 
        [SerializeField] private Celeste.Events.Event chessBoardTilemapChanged;

        [NonSerialized] private readonly List<ChessBoardObjectView> chessBoardObjectViews = new();
        
        #endregion

        public void Setup()
        {
            hitBoxCollider.enabled = false;
        }

        public void Shutdown()
        {
            foreach (var view in chessBoardObjectViews)
            {
                view.Shutdown();
            }
            
            chessBoardObjectViews.Clear();
        }

        private void CreateChessBoardObjectView(ChessBoardObjectInstance chessBoardObjectInstance)
        {
            // TODO: Change this to use Object Pooling 
            ChessBoardObjectView viewInstance = Instantiate(chessBoardObjectView, piecesViewRoot, false);
            viewInstance.Hookup(chessBoardObjectInstance, IRuntimeAddedContext.Empty);
            chessBoardObjectViews.Add(viewInstance);
        }

        private void AddChessBoardTile(Vector2Int position, TileBase tile)
        {
            chessBoardTilemap.SetTile(position.ToVector3Int(), tile);
            chessBoardTilemap.CompressBounds();
            chessBoardTilemapChanged.Invoke();
        }

        private void RemoveChessBoardTile(Vector2Int position)
        {
            chessBoardTilemap.SetTile(position.ToVector3Int(), null);
            chessBoardTilemap.CompressBounds();
            chessBoardTilemapChanged.Invoke();
        }
        
        #region Callbacks

        public void OnChessBoardTileAdded(ChessBoardTileAddedArgs args)
        {
            AddChessBoardTile(args.Instance.Position, args.Instance.Tile);
        }

        public void OnChessBoardTileRemoved(ChessBoardTileRemovedArgs args)
        {
            RemoveChessBoardTile(args.BoardPosition);
        }

        public void OnChessBoardObjectSelectionChanged(ChessBoardObjectSelectionChangedArgs args)
        {
            hitBoxCollider.enabled = args.NewSelectedObject != null;
        }

        public void OnChessBoardObjectAdded(ChessBoardObjectAddedArgs args)
        {
            CreateChessBoardObjectView(args.AddedObject);
        }

        public void OnChessBoardObjectRemoved(ChessBoardObjectRemovedArgs args)
        {
            int viewIndex = chessBoardObjectViews.FindIndex(x => x.Instance == args.RemovedInstance);
            if (viewIndex >= 0)
            {
                // TODO: Change this to use Object Pooling
                ChessBoardObjectView viewInstance = chessBoardObjectViews[viewIndex];
                chessBoardObjectViews.RemoveAt(viewIndex);
                Destroy(viewInstance.gameObject);
            }
        }

        public void OnChessBoardBackgroundPressed(InputState inputState)
        {
            ClearSelectedChessBoardObjectCommand clearSelectionCommands = new ClearSelectedChessBoardObjectCommand();
            chessBoardCommandExecutor.QueueCommand(clearSelectionCommands);
        }

        public void OnChessBoardPressed(InputState inputState)
        {
            ClearSelectedChessBoardObjectCommand clearSelectionCommands = new ClearSelectedChessBoardObjectCommand();
            chessBoardCommandExecutor.QueueCommand(clearSelectionCommands);
        }

        public void OnChessBoardMovementActionPressed(InputState inputState)
        {
            Vector2Int position = chessBoardTilemap.WorldToCell(inputState.PointerWorldPosition).ToVector2Int();
            MoveSelectedChessBoardObjectToCommand moveSelectedChessBoardObjectToCommand = new MoveSelectedChessBoardObjectToCommand(position);
            chessBoardCommandExecutor.QueueCommand(moveSelectedChessBoardObjectToCommand);
        }

        public void OnChessBoardAttackActionPressed(InputState inputState)
        {
            Vector2Int position = chessBoardTilemap.WorldToCell(inputState.PointerWorldPosition).ToVector2Int();
            AttackWithSelectedChessBoardObjectCommand attackWithSelectedChessBoardObjectCommand = new AttackWithSelectedChessBoardObjectCommand(position);
            chessBoardCommandExecutor.QueueCommand(attackWithSelectedChessBoardObjectCommand);
        }
        
        #endregion
    }
}