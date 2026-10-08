using System.Threading;
using Cysharp.Threading.Tasks;
using QC.Match.Objects;

namespace QC.Match.Commands
{
    public readonly struct RemoveBoardObjectCommand : ICommand<BoardCommandArgs>
    {
        private readonly BoardObjectInstance InstanceToRemove;

        public RemoveBoardObjectCommand(BoardObjectInstance instanceToRemove)
        {
            InstanceToRemove = instanceToRemove;
        }

        public UniTask<CommandState> Execute(BoardCommandArgs args, CancellationToken token)
        {
            return RemoveCommandUtilities.RemoveBoardObject(args, InstanceToRemove);
        }

        public override string ToString()
        {
            return $"{nameof(RemoveBoardObjectCommand)} - Instance Id: {InstanceToRemove.InstanceId}";
        }
    }
}