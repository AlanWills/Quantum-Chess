using System.Runtime.CompilerServices;

namespace QC.Match.Functors
{
    public class MatchColourFunctor : IColourFunctor
    {
        private readonly PieceColour colourToMatch;

        public MatchColourFunctor(PieceColour colourToMatch)
        {
            this.colourToMatch = colourToMatch;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsAMatch(PieceColour colour)
        {
            return colour == colourToMatch;
        }
    }
}