namespace QC.Match.Commands
{
    public readonly struct ClearChessBoardCommand : ICommand<ChessBoardCommandArgs>
    {
        public CommandState Execute(ChessBoardCommandArgs args)
        {
            args.ChessBoard.Clear();

            return CommandState.Completed;
        }
    }
}