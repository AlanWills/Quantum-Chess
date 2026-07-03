using Celeste.Components;
using QC.Match.Components;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Views
{
    public class ChessPieceSpriteController : InterfaceController<IChessPiece, ChessBoardObjectComponent, ChessBoardObjectInstance>
    {
        #region Properties and Fields
        
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private ChessPieceVisualLookup visualLookup;
        
        #endregion
        
        protected override void DoHookup(IRuntimeAddedContext context)
        {
            SetSpriteFromType();
        }

        protected override void DoShutdown() { }

        protected override void OnInterfaceDataChanged()
        {
            SetSpriteFromType();
        }

        private void SetSpriteFromType()
        {
            ChessPieceColour currentColour = Interface.GetColour();
            ChessPieceType currentPieceType = ChessPieceExtensions.GetType(Interface);
            spriteRenderer.sprite = visualLookup.GetSprite(currentColour, currentPieceType);
        }
    }
}