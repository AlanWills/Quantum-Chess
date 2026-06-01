using QC.Match.Objects;

namespace QC.Match.Commands
{
    public readonly struct ChessBoardCommandArgs
    {
        public readonly MatchState MatchState;
        public readonly ChessBoardCommandExecutor CommandExecutor; 
        public readonly ChessBoard ChessBoard;
        public readonly ChessBoardObjectSelection Selection;

        public ChessBoardCommandArgs(
            MatchState matchState,
            ChessBoardCommandExecutor commandExecutor,
            ChessBoard chessBoard,
            ChessBoardObjectSelection selection)
        {
            MatchState = matchState;
            CommandExecutor = commandExecutor;
            ChessBoard = chessBoard;
            Selection = selection;
        }
    }
}