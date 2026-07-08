using System.Threading;
using Cysharp.Threading.Tasks;
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

        public UniTask<CommandState> Execute(ChessBoardCommandArgs args, CancellationToken token)
        {
            return RemoveCommandUtilities.RemoveChessBoardObject(args, InstanceToRemove);
        }
    }
}