using System;
using System.ComponentModel;
using Celeste.Components;

namespace QC.Match.Components
{
    [DisplayName("Interactable")]
    public class InteractableComponent : ChessBoardObjectComponent, IInteractable
    {
        #region Properties and Fields

        [NonSerialized] private bool isInteractable = true;
        
        #endregion
        
        public bool IsInteractable(Instance instance)
        {
            return isInteractable;
        }
    }
}