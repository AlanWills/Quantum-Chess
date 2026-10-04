using System.Threading;
using Cysharp.Threading.Tasks;
using QC.Match.Objects;

namespace QC.Match.Commands
{
    public readonly struct RemoveChessBoardObjectCommand : ICommand<BoardCommandArgs>
    {
        private readonly BoardObjectInstance InstanceToRemove;

        public RemoveChessBoardObjectCommand(BoardObjectInstance instanceToRemove)
        {
            InstanceToRemove = instanceToRemove;
        }

        public UniTask<CommandState> Execute(BoardCommandArgs args, CancellationToken token)
        {
            return RemoveCommandUtilities.RemoveChessBoardObject(args, InstanceToRemove);
        }

        public override string ToString()
        {
            return $"{nameof(RemoveChessBoardObjectCommand)} - Instance Id: {InstanceToRemove.InstanceId}";
        }
    }
}