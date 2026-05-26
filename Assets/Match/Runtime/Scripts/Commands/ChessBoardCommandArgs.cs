using QC.Match.Objects;

namespace QC.Match.Commands
{
    public readonly struct ChessBoardCommandArgs
    {
        public readonly ChessBoard ChessBoard;
        public readonly ChessBoardObjectSelection Selection; 

        public ChessBoardCommandArgs(
            ChessBoard chessBoard,
            ChessBoardObjectSelection selection)
        {
            ChessBoard = chessBoard;
            Selection = selection;
        }
    }
}