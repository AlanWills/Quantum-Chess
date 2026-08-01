namespace QC.Match
{
    public interface IColourFunctor
    {
        bool IsAMatch(ChessPieceColour colour);
        IColourFunctor Invert();
    }
}