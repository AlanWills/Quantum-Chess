using Celeste.Components;
using QC.Match.Components;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Views
{
    public class ChessPieceSpriteController : InterfaceController<IPiece, BoardObjectComponent, BoardObjectInstance>
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
            PieceColour currentColour = Interface.GetPieceColour();
            PieceType currentPieceType = ChessPieceExtensions.GetPieceType(Interface);
            spriteRenderer.sprite = visualLookup.GetSprite(currentColour, currentPieceType);
        }
    }
}