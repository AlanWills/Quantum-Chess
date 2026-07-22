using Celeste.Components;
using QC.Match.Objects;

namespace QC.Match.AI.Internals
{
    public readonly struct Piece
    {
        public bool IsValid => Instance != null && PieceType != ChessPieceType.None && PieceColour != ChessPieceColour.Unknown && ActionProvider.IsValid;
           
        public readonly BoardObjectInstance Instance;
        public readonly ChessPieceType PieceType;
        public readonly ChessPieceColour PieceColour;
        public readonly int Value;
        public readonly InterfaceHandle<IActionProvider> ActionProvider;

        public Piece(
            BoardObjectInstance instance, 
            ChessPieceType pieceType,
            ChessPieceColour pieceColour,
            int value,
            InterfaceHandle<IActionProvider> actionProvider)
        {
            Instance = instance;
            PieceType = pieceType;
            PieceColour = pieceColour;
            Value = value;
            ActionProvider = actionProvider;
        }
    }
}