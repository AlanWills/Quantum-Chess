namespace QC.Match.Functors
{
    public readonly struct MatchColourFunctor : IColourFunctor
    {
        private readonly ChessPieceColour colourToMatch;

        public MatchColourFunctor(ChessPieceColour colourToMatch)
        {
            this.colourToMatch = colourToMatch;
        }

        public bool IsAMatch(ChessPieceColour colour)
        {
            return colour == colourToMatch;
        }

        public IColourFunctor Invert()
        {
            return new DoNotMatchColourFunctor(colourToMatch);
        }
    }
}