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
        }
        
        #region Callbacks

        public void OnChessBoardTileAdded(ChessBoardTileAddedArgs args)
        {
            AddChessBoardTile(args.BoardPosition, args.Tile);
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

        public void OnChessBoardPressed(InputState inputState)
        {
            Vector3Int boardPosition = chessBoardTilemap.WorldToCell(inputState.PointerWorldPosition);
            
            // Board Positions are 1-indexed
            boardPosition.x += 1;
            boardPosition.y += 1;
            
            MoveSelectedChessBoardObjectToCommand moveSelectedChessBoardObjectToCommand =
                new MoveSelectedChessBoardObjectToCommand(boardPosition.ToVector2Int());
            chessBoardCommandExecutor.QueueCommand(moveSelectedChessBoardObjectToCommand);
        }
        
        #endregion
    }
}