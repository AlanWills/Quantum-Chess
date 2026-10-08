namespace QC.Match.Functors
{
    public readonly struct MatchColourFunctor : IColourFunctor
    {
        private readonly PieceColour colourToMatch;

        public MatchColourFunctor(PieceColour colourToMatch)
        {
            this.colourToMatch = colourToMatch;
        }

        public bool IsAMatch(PieceColour colour)
        {
            return colour == colourToMatch;
        }

        public IColourFunctor Invert()
        {
            return new DoNotMatchColourFunctor(colourToMatch);
        }
    }
}