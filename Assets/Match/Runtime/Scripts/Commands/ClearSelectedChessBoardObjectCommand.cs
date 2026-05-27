namespace QC.Match.Commands
{
    public struct ClearSelectedChessBoardObjectCommand : ICommand<ChessBoardCommandArgs>
    {
        public CommandState Execute(ChessBoardCommandArgs args)
        {
            args.Selection.CurrentSelectedObject = null;
            return CommandState.Completed;
        }
    }
}