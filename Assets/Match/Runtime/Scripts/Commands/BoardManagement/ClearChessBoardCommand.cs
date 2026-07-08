using System.Threading;
using Cysharp.Threading.Tasks;

namespace QC.Match.Commands
{
    public readonly struct ClearChessBoardCommand : ICommand<ChessBoardCommandArgs>
    {
        public UniTask<CommandState> Execute(ChessBoardCommandArgs args, CancellationToken token)
        {
            args.ChessBoard.Clear();

            return UniTask.FromResult(CommandState.Completed);
        }
    }
}