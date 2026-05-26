using Celeste.Components;
using Celeste.FX;
using QC.Match.Components;
using QC.Match.Events;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Views
{
    public class ChessBoardObjectView : ComponentContainerController<ChessBoardObjectInstance, ChessBoardObjectComponent>
    {
        #region Properties and Fields
        
        [SerializeField] private Outline selectedOutline;

        #endregion

        protected override void DoHookup(IRuntimeAddedContext context)
        {
            base.DoHookup(context);
            
            RefreshSelectedOutline(null);
        }

        private void RefreshSelectedOutline(ChessBoardObjectInstance currentSelectedInstance)
        {
            selectedOutline.enabled = Instance == currentSelectedInstance;
        }
        
        #region Callbacks

        public void OnSelectionChanged(ChessBoardObjectSelectionChangedArgs args)
        {
            RefreshSelectedOutline(args.newSelectedObject);
        }
        
        #endregion
    }
}