using System.Threading;
using Cysharp.Threading.Tasks;

namespace QC.Match.Commands
{
    public readonly struct EmptyCommand : ICommand<ChessBoardCommandArgs>
    {
        public UniTask<CommandState> Execute(ChessBoardCommandArgs args, CancellationToken cancellationToken)
        {
            return UniTask.FromResult(CommandState.Completed);
        }
    }
}