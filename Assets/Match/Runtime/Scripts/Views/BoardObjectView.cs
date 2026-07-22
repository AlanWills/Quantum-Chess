using Celeste.Components;
using Celeste.FX;
using QC.Match.Components;
using QC.Match.Events;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Views
{
    public class BoardObjectView : ComponentContainerController<BoardObjectInstance, BoardObjectComponent>
    {
        #region Properties and Fields
        
        [SerializeField] private Outline selectedOutline;

        #endregion

        protected override void DoHookup(IRuntimeAddedContext context)
        {
            base.DoHookup(context);
            
            RefreshSelectedOutline(null);
        }

        private void RefreshSelectedOutline(BoardObjectInstance currentSelectedInstance)
        {
            selectedOutline.enabled = Instance == currentSelectedInstance;
        }
        
        #region Callbacks

        public void OnSelectionChanged(BoardObjectSelectionChangedArgs args)
        {
            RefreshSelectedOutline(args.NewSelectedObject);
        }
        
        #endregion
    }
}