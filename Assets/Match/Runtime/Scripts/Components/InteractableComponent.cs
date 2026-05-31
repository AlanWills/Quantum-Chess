using System;
using System.ComponentModel;
using Celeste.Components;

namespace QC.Match.Components
{
    [DisplayName("Interactable")]
    public class InteractableComponent : ChessBoardObjectComponent, IInteractable
    {
        #region Data

        [Serializable]
        private class Data : ComponentData
        {
            [NonSerialized] public bool isInteractable;
        }
        
        #endregion

        public override ComponentData CreateData()
        {
            return new Data();
        }

        public bool IsInteractable(Instance instance)
        {
            return (instance.data as Data).isInteractable;
        }

        public void SetInteractable(Instance instance, bool interactable)
        {
            Data data = instance.data as Data;
            if (data.isInteractable == interactable)
            {
                return;
            }
            
            data.isInteractable = interactable;
            instance.events.ComponentDataChanged.Invoke();
        }
    }
}