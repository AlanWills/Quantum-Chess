using QC.Match.Objects;

namespace QC.Match.Events
{
    public readonly struct ChessBoardTileAddedArgs
    {
        public readonly ChessBoardTileInstance Instance;

        public ChessBoardTileAddedArgs(ChessBoardTileInstance instance)
        {
            Instance = instance;
        }
    }
}