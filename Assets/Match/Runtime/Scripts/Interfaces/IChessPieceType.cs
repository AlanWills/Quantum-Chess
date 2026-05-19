using Celeste.Components;

namespace QC.Match
{
    public interface IChessPieceType
    {
        ChessPieceType GetPieceType(Instance instance);
    }
}