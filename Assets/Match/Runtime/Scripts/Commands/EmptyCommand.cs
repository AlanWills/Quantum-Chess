using System.Threading;
using Cysharp.Threading.Tasks;

namespace QC.Match.Commands
{
    public readonly struct EmptyCommand : ICommand<BoardCommandArgs>
    {
        public UniTask<CommandState> Execute(BoardCommandArgs args, CancellationToken cancellationToken)
        {
            return UniTask.FromResult(CommandState.Completed);
        }
    }
}