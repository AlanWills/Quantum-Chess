using Cysharp.Threading.Tasks;

namespace QC.Match.Commands
{
    public struct ClearSelectedChessBoardObjectCommand : ICommand<ChessBoardCommandArgs>
    {
        public UniTask<CommandState> Execute(ChessBoardCommandArgs args)
        {
            args.Selection.CurrentSelectedObject = null;
            return UniTask.FromResult(CommandState.Completed);
        }
    }
}