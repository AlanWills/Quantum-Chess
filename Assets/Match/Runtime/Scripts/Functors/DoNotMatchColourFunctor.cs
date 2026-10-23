using System.Runtime.CompilerServices;

namespace QC.Match.Functors
{
    public class DoNotMatchColourFunctor : IColourFunctor
    {
        private readonly PieceColour colourToNotMatch;

        public DoNotMatchColourFunctor(PieceColour colourToNotMatch)
        {
            this.colourToNotMatch = colourToNotMatch;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsAMatch(PieceColour colour)
        {
            return colour != colourToNotMatch;
        }
    }
}