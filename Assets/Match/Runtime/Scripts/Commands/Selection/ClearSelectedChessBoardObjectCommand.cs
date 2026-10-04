using System.Threading;
using Cysharp.Threading.Tasks;

namespace QC.Match.Commands
{
    public struct ClearSelectedChessBoardObjectCommand : ICommand<BoardCommandArgs>
    {
        public UniTask<CommandState> Execute(BoardCommandArgs args, CancellationToken token)
        {
            args.Selection.CurrentSelectedObject = null;
            return UniTask.FromResult(CommandState.Completed);
        }

        public override string ToString()
        {
            return $"{nameof(ClearSelectedChessBoardObjectCommand)}";
        }
    }
}