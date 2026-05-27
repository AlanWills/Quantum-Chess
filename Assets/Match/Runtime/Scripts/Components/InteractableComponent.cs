using System;
using System.ComponentModel;
using Celeste.Components;

namespace QC.Match.Components
{
    [DisplayName("Interactable")]
    public class InteractableComponent : ChessBoardObjectComponent, IInteractable
    {
        #region Properties and Fields

        [NonSerialized] private bool isInteractable;
        
        #endregion
        
        public bool IsInteractable(Instance instance)
        {
            return isInteractable;
        }

        public void SetInteractable(Instance instance, bool interactable)
        {
            if (isInteractable == interactable)
            {
                return;
            }
            
            isInteractable = interactable;
            instance.events.ComponentDataChanged.Invoke();
        }
    }
}