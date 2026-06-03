using QC.Match.Objects;

namespace QC.Match.Commands
{
    public readonly struct RemoveChessBoardObjectCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly ChessBoardObjectInstance InstanceToRemove;

        public RemoveChessBoardObjectCommand(ChessBoardObjectInstance instanceToRemove)
        {
            InstanceToRemove = instanceToRemove;
        }

        public CommandState Execute(ChessBoardCommandArgs args)
        {
            return args.ChessBoard.RemoveChessBoardObject(InstanceToRemove)
                ? CommandState.Completed
                : CommandState.Abandoned;
        }
    }
}