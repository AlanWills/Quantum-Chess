using QC.Match.Objects;

namespace QC.Match.Events
{
    public readonly struct BoardTileAddedArgs
    {
        public readonly BoardTileInstance Instance;

        public BoardTileAddedArgs(BoardTileInstance instance)
        {
            Instance = instance;
        }
    }
}