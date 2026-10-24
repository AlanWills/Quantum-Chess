using Celeste.Components;
using QC.Match.Objects;

namespace QC.Match.AI.Internals
{
    public readonly struct Piece
    {
        public readonly bool IsValid;
        public readonly BoardObjectInstance Instance;
        public readonly PieceType PieceType;
        public readonly PieceColour PieceColour;
        public readonly int Value;
        public readonly InterfaceHandle<IActionProvider> ActionProvider;

        public Piece(
            BoardObjectInstance instance, 
            PieceType pieceType,
            PieceColour pieceColour,
            int value,
            InterfaceHandle<IActionProvider> actionProvider)
        {
            Instance = instance;
            PieceType = pieceType;
            PieceColour = pieceColour;
            Value = value;
            ActionProvider = actionProvider;
            IsValid = Instance != null && PieceType != PieceType.None && PieceColour != PieceColour.None && ActionProvider.IsValid;
        }
    }
}