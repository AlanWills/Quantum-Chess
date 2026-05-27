using QC.Match.Objects;

namespace QC.Match.Commands
{
    public readonly struct ChessBoardCommandArgs
    {
        public readonly ChessBoardCommandExecutor CommandExecutor; 
        public readonly ChessBoard ChessBoard;
        public readonly ChessBoardObjectSelection Selection;

        public ChessBoardCommandArgs(
            ChessBoardCommandExecutor commandExecutor,
            ChessBoard chessBoard,
            ChessBoardObjectSelection selection)
        {
            CommandExecutor = commandExecutor;
            ChessBoard = chessBoard;
            Selection = selection;
        }
    }
}