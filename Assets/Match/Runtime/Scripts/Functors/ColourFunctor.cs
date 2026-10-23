using System.Runtime.CompilerServices;

namespace QC.Match.Functors
{
    public class ColourFunctor : IColourFunctor
    {
        private readonly MatchColourFunctor matchColourFunctor;
        private readonly DoNotMatchColourFunctor doNotMatchColourFunctor;
        private IColourFunctor activeFunctor;
        private IColourFunctor inactiveFunctor;

        public static ColourFunctor Match(PieceColour colour)
        {
            ColourFunctor colourFunctor = new ColourFunctor
            {
                activeFunctor = new MatchColourFunctor(colour),
                inactiveFunctor = new DoNotMatchColourFunctor(colour)
            };

            return colourFunctor;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsAMatch(PieceColour colour)
        {
            return activeFunctor.IsAMatch(colour);
        }

        public ColourFunctor Invert()
        {
            (inactiveFunctor, activeFunctor) = (activeFunctor, inactiveFunctor);
            return this;
        }
    }
}