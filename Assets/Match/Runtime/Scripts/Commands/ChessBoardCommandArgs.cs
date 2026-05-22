using QC.Match.Objects;

namespace QC.Match.Commands
{
    public readonly struct ChessBoardCommandArgs
    {
        public readonly ChessBoard chessBoard;

        public ChessBoardCommandArgs(ChessBoard chessBoard)
        {
            this.chessBoard = chessBoard;
        }
    }
}