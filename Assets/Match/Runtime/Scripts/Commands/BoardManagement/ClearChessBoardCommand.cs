using System.Threading;
using Cysharp.Threading.Tasks;

namespace QC.Match.Commands
{
    public readonly struct ClearChessBoardCommand : ICommand<BoardCommandArgs>
    {
        public UniTask<CommandState> Execute(BoardCommandArgs args, CancellationToken token)
        {
            args.Board.Clear();

            return UniTask.FromResult(CommandState.Completed);
        }

        public override string ToString()
        {
            return $"{nameof(ClearChessBoardCommand)}";
        }
    }
}