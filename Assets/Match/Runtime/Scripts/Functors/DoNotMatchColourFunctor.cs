namespace QC.Match.Functors
{
    public readonly struct DoNotMatchColourFunctor : IColourFunctor
    {
        private readonly ChessPieceColour colourToNotMatch;

        public DoNotMatchColourFunctor(ChessPieceColour colourToNotMatch)
        {
            this.colourToNotMatch = colourToNotMatch;
        }
        
        public bool IsAMatch(ChessPieceColour colour)
        {
            return colour != colourToNotMatch;
        }

        public IColourFunctor Invert()
        {
            return new DoNotMatchColourFunctor(colourToNotMatch);
        }
    }
}