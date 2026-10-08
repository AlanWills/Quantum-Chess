namespace QC.Match.Functors
{
    public readonly struct DoNotMatchColourFunctor : IColourFunctor
    {
        private readonly PieceColour colourToNotMatch;

        public DoNotMatchColourFunctor(PieceColour colourToNotMatch)
        {
            this.colourToNotMatch = colourToNotMatch;
        }
        
        public bool IsAMatch(PieceColour colour)
        {
            return colour != colourToNotMatch;
        }

        public IColourFunctor Invert()
        {
            return new DoNotMatchColourFunctor(colourToNotMatch);
        }
    }
}