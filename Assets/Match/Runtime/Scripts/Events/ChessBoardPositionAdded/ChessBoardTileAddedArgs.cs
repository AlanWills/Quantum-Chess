using QC.Match.Objects;

namespace QC.Match.Events
{
    public readonly struct ChessBoardTileAddedArgs
    {
        public readonly ChessBoardTileInstance Tile;

        public ChessBoardTileAddedArgs(ChessBoardTileInstance tile)
        {
            Tile = tile;
        }
    }
}