using Celeste.Components;
using Celeste.Input;
using QC.Match.Commands;
using QC.Match.Components;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Input
{
    public class ChessBoardObjectInputController : InterfaceController<IInteractable, ChessBoardObjectComponent, ChessBoardObjectInstance>
    {
        #region Properties and Fields

        private bool IsInteractable => Interface.IsValid && Interface.iFace.IsInteractable(Interface.instance);
        
        [SerializeField] private ChessBoardCommandExecutor commandExecutor;
        
        #endregion
        
        protected override void DoHookup(IRuntimeAddedContext context)
        {
        }

        protected override void DoShutdown()
        {
        }
        
        #region Callbacks

        public void OnPointerDown(InputState inputState)
        {
            if (!IsInteractable)
            {
                return;
            }

            SelectChessBoardObjectCommand selectChessBoardObjectCommand = new SelectChessBoardObjectCommand(Instance);
            commandExecutor.QueueCommand(selectChessBoardObjectCommand);
        }
        
        #endregion
    }
}