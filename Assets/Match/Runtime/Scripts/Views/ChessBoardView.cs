using System;
using System.Collections.Generic;
using Celeste.Components;
using Celeste.Input;
using QC.Match.Event;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Views
{
    public class ChessBoardView : MonoBehaviour
    {
        #region Properties and Fields
        
        [SerializeField] private ChessBoardObjectView chessBoardObjectView;
        [SerializeField] private Transform viewRoot;

        [NonSerialized] private readonly List<ChessBoardObjectView> chessBoardObjectViews = new();
        
        #endregion

        public void Shutdown()
        {
            foreach (var chessBoardObjectView in chessBoardObjectViews)
            {
                chessBoardObjectView.Shutdown();
            }
            
            chessBoardObjectViews.Clear();
        }

        private void CreateChessBoardObjectView(ChessBoardObjectInstance chessBoardObjectInstance)
        {
            ChessBoardObjectView viewInstance = Instantiate(chessBoardObjectView, viewRoot, false);
            viewInstance.Hookup(chessBoardObjectInstance, IRuntimeAddedContext.Empty);
        }
        
        #region Callbacks

        public void OnChessBoardObjectAdded(ChessBoardObjectAddedArgs args)
        {
            CreateChessBoardObjectView(args.AddedObject);
        }

        public void OnChessBoardPressed(InputState inputState)
        {
            
        }
        
        #endregion
    }
}