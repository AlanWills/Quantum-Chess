using System;
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
        [SerializeField] private Collider2D hitBoxCollider;
        
        #endregion

        protected override void DoHookup(IRuntimeAddedContext context)
        {
            UpdateHitBoxCollider();
        }

        protected override void DoShutdown()
        {
            if (hitBoxCollider != null)
            {
                hitBoxCollider.enabled = false;
            }
        }

        protected override void OnInterfaceDataChanged()
        {
            base.OnInterfaceDataChanged();
            
            UpdateHitBoxCollider();
        }

        private void UpdateHitBoxCollider()
        {
            hitBoxCollider.enabled = IsInteractable;
        }

        #region Callbacks

        public void OnPointerDown(InputState inputState)
        {
            if (!IsInteractable)
            {
                UnityEngine.Debug.LogAssertion($"{nameof(ChessBoardObjectInputController)} registered hit despite it not being interactable.", gameObject);
                return;
            }
            
            SelectChessBoardObjectCommand selectChessBoardObjectCommand = new SelectChessBoardObjectCommand(Instance);
            commandExecutor.QueueCommand(selectChessBoardObjectCommand);
        }
        
        #endregion
    }
}