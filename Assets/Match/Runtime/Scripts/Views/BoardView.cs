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

        [SerializeField] private GameObjectAllocator_List chessBoardObjectViewAllocator;
        [SerializeField] private BoardCommandExecutor boardCommandExecutor;
        [SerializeField] private MatchState matchState;
        [SerializeField] private Transform piecesViewRoot;
        [SerializeField] private Tilemap boardTilemap;
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

        private void CreateBoardObjectView(BoardObjectInstance boardObjectInstance)
        {
            GameObject chessBoardObjectGameObject = chessBoardObjectViewAllocator.AllocateWithResizeIfNecessary();
            chessBoardObjectGameObject.transform.SetParent(piecesViewRoot);
            BoardObjectView viewInstance = chessBoardObjectGameObject.GetComponent<BoardObjectView>();
            UnityEngine.Debug.Assert(viewInstance != null, $"Failed to find instance of script {nameof(BoardObjectView)} on chess board object from allocator {chessBoardObjectViewAllocator.name}.");
            viewInstance.Hookup(boardObjectInstance, IRuntimeAddedContext.Empty);
            boardObjectViews.Add(viewInstance);
        }

        private void AddBoardTile(Vector2Int position, TileBase tile)
        {
            boardTilemap.SetTile(position.ToVector3Int(), tile);
            boardTilemap.CompressBounds();
            boardTilemapChanged.Invoke();
        }

        private void RemoveBoardTile(Vector2Int position)
        {
            boardTilemap.SetTile(position.ToVector3Int(), null);
            boardTilemap.CompressBounds();
            boardTilemapChanged.Invoke();
        }
        
        #region Callbacks

        public void OnBoardTileAdded(BoardTileAddedArgs args)
        {
            AddBoardTile(args.Instance.Position, args.Instance.Tile);
        }

        public void OnBoardTileRemoved(BoardTileRemovedArgs args)
        {
            RemoveBoardTile(args.BoardPosition);
        }

        public void OnBoardObjectSelectionChanged(BoardObjectSelectionChangedArgs args)
        {
            hitBoxCollider.enabled = args.NewSelectedObject != null;
        }

        public void OnBoardObjectAdded(BoardObjectAddedArgs args)
        {
            CreateBoardObjectView(args.AddedObject);
        }

        public void OnBoardObjectRemoved(BoardObjectRemovedArgs args)
        {
            int viewIndex = boardObjectViews.FindIndex(x => x.Instance == args.RemovedInstance);
            if (viewIndex >= 0)
            {
                BoardObjectView viewInstance = boardObjectViews[viewIndex];
                boardObjectViews.RemoveAt(viewIndex);
                chessBoardObjectViewAllocator.Deallocate(viewInstance.gameObject);
            }
        }

        public void OnBoardBackgroundPressed(InputState inputState)
        {
            ClearSelectedBoardObjectCommand clearSelectionCommands = new ClearSelectedBoardObjectCommand();
            boardCommandExecutor.QueueCommand(clearSelectionCommands);
        }

        public void OnBoardPressed(InputState inputState)
        {
            ClearSelectedBoardObjectCommand clearSelectionCommands = new ClearSelectedBoardObjectCommand();
            boardCommandExecutor.QueueCommand(clearSelectionCommands);
        }

        public void OnBoardMovementActionPressed(InputState inputState)
        {
            Vector2Int position = boardTilemap.WorldToCell(inputState.PointerWorldPosition).ToVector2Int();
            MoveSelectedBoardObjectToCommand moveSelectedBoardObjectToCommand = new MoveSelectedBoardObjectToCommand(position);
            boardCommandExecutor.QueueCommand(moveSelectedBoardObjectToCommand);
        }

        public void OnBoardAttackActionPressed(InputState inputState)
        {
            Vector2Int position = boardTilemap.WorldToCell(inputState.PointerWorldPosition).ToVector2Int();
            AttackWithSelectedBoardObjectCommand attackWithSelectedBoardObjectCommand = new AttackWithSelectedBoardObjectCommand(position);
            boardCommandExecutor.QueueCommand(attackWithSelectedBoardObjectCommand);
        }
        
        #endregion
    }
}