using Celeste.Components;

namespace QC.Match
{
    public interface IChessPiece
    {
        ChessPieceColour GetPieceColour(Instance instance);
        ChessPieceType GetPieceType(Instance instance);
        void SetVerticalDirection(Instance instance, ChessPieceVerticalDirection verticalDirection);
    }
    
    public interface IModifiableChessPiece : IChessPiece
    {
        void SetColour(Instance instance, ChessPieceColour colour);
        void SetType(Instance instance, ChessPieceType type);
    }
}