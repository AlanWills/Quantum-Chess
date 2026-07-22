using System;
using System.Collections.Generic;
using Celeste.Components;
using Celeste.Input;
using Celeste.Maths;
using Celeste.Memory;
using QC.Match.Commands;
using QC.Match.Events;
using QC.Match.Objects;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace QC.Match.Views
{
    public class BoardView : MonoBehaviour
    {
        #region Properties and Fields

        [SerializeField] private GameObjectAllocator chessBoardObjectViewAllocator;
        [SerializeField] private BoardCommandExecutor boardCommandExecutor;
        [SerializeField] private MatchState matchState;
        [SerializeField] private Transform piecesViewRoot;
        [SerializeField] private Tilemap chessBoardTilemap;
        [SerializeField] private Collider2D hitBoxCollider;

        [Header("Events")] 
        [SerializeField] private Celeste.Events.Event boardTilemapChanged;

        [NonSerialized] private readonly List<BoardObjectView> boardObjectViews = new();
        
        #endregion

        public void Setup()
        {
            hitBoxCollider.enabled = false;
        }

        public void Shutdown()
        {
            foreach (var view in boardObjectViews)
            {
                view.Shutdown();
            }
            
            boardObjectViews.Clear();
        }

        private void CreateChessBoardObjectView(BoardObjectInstance boardObjectInstance)
        {
            GameObject chessBoardObjectGameObject = chessBoardObjectViewAllocator.AllocateWithResizeIfNecessary();
            chessBoardObjectGameObject.transform.SetParent(piecesViewRoot);
            BoardObjectView viewInstance = chessBoardObjectGameObject.GetComponent<BoardObjectView>();
            UnityEngine.Debug.Assert(viewInstance != null, $"Failed to find instance of script {nameof(BoardObjectView)} on chess board object from allocator {chessBoardObjectViewAllocator.name}.");
            viewInstance.Hookup(boardObjectInstance, IRuntimeAddedContext.Empty);
            boardObjectViews.Add(viewInstance);
        }

        private void AddChessBoardTile(Vector2Int position, TileBase tile)
        {
            chessBoardTilemap.SetTile(position.ToVector3Int(), tile);
            chessBoardTilemap.CompressBounds();
            boardTilemapChanged.Invoke();
        }

        private void RemoveChessBoardTile(Vector2Int position)
        {
            chessBoardTilemap.SetTile(position.ToVector3Int(), null);
            chessBoardTilemap.CompressBounds();
            boardTilemapChanged.Invoke();
        }
        
        #region Callbacks

        public void OnChessBoardTileAdded(BoardTileAddedArgs args)
        {
            AddChessBoardTile(args.Instance.Position, args.Instance.Tile);
        }

        public void OnChessBoardTileRemoved(BoardTileRemovedArgs args)
        {
            RemoveChessBoardTile(args.BoardPosition);
        }

        public void OnChessBoardObjectSelectionChanged(BoardObjectSelectionChangedArgs args)
        {
            hitBoxCollider.enabled = args.NewSelectedObject != null;
        }

        public void OnChessBoardObjectAdded(BoardObjectAddedArgs args)
        {
            CreateChessBoardObjectView(args.AddedObject);
        }

        public void OnChessBoardObjectRemoved(BoardObjectRemovedArgs args)
        {
            int viewIndex = boardObjectViews.FindIndex(x => x.Instance == args.RemovedInstance);
            if (viewIndex >= 0)
            {
                BoardObjectView viewInstance = boardObjectViews[viewIndex];
                boardObjectViews.RemoveAt(viewIndex);
                chessBoardObjectViewAllocator.Deallocate(viewInstance.gameObject);
            }
        }

        public void OnChessBoardBackgroundPressed(InputState inputState)
        {
            ClearSelectedChessBoardObjectCommand clearSelectionCommands = new ClearSelectedChessBoardObjectCommand();
            boardCommandExecutor.QueueCommand(clearSelectionCommands);
        }

        public void OnChessBoardPressed(InputState inputState)
        {
            ClearSelectedChessBoardObjectCommand clearSelectionCommands = new ClearSelectedChessBoardObjectCommand();
            boardCommandExecutor.QueueCommand(clearSelectionCommands);
        }

        public void OnChessBoardMovementActionPressed(InputState inputState)
        {
            Vector2Int position = chessBoardTilemap.WorldToCell(inputState.PointerWorldPosition).ToVector2Int();
            MoveSelectedChessBoardObjectToCommand moveSelectedChessBoardObjectToCommand = new MoveSelectedChessBoardObjectToCommand(position);
            boardCommandExecutor.QueueCommand(moveSelectedChessBoardObjectToCommand);
        }

        public void OnChessBoardAttackActionPressed(InputState inputState)
        {
            Vector2Int position = chessBoardTilemap.WorldToCell(inputState.PointerWorldPosition).ToVector2Int();
            AttackWithSelectedChessBoardObjectCommand attackWithSelectedChessBoardObjectCommand = new AttackWithSelectedChessBoardObjectCommand(position);
            boardCommandExecutor.QueueCommand(attackWithSelectedChessBoardObjectCommand);
        }
        
        #endregion
    }
}