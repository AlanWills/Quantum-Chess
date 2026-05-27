using System;
using System.Collections.Generic;
using Celeste.Components;
using Celeste.Input;
using Celeste.Maths;
using QC.Match.Commands;
using QC.Match.Event;
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
        [SerializeField] private Transform viewRoot;
        [SerializeField] private Tilemap chessBoardTilemap;

        [NonSerialized] private readonly List<ChessBoardObjectView> chessBoardObjectViews = new();
        
        #endregion

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
            ChessBoardObjectView viewInstance = Instantiate(chessBoardObjectView, viewRoot, false);
            viewInstance.Hookup(chessBoardObjectInstance, IRuntimeAddedContext.Empty);
            chessBoardObjectViews.Add(viewInstance);
        }
        
        #region Callbacks

        public void OnChessBoardObjectAdded(ChessBoardObjectAddedArgs args)
        {
            CreateChessBoardObjectView(args.AddedObject);
        }

        public void OnChessBoardPressed(InputState inputState)
        {
            Vector3Int boardPosition = chessBoardTilemap.WorldToCell(inputState.PointerWorldPosition) - chessBoardTilemap.origin;
            
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