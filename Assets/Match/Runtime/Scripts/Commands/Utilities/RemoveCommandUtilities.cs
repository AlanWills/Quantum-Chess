using Cysharp.Threading.Tasks;
using QC.Match.Objects;

namespace QC.Match.Commands
{
    public static class RemoveCommandUtilities
    {
        public static UniTask<CommandState> RemoveChessBoardObject(
            ChessBoardCommandArgs args,
            ChessBoardObjectInstance objectToRemove)
        {
            CommandState commandState = args.ChessBoard.RemoveChessBoardObject(objectToRemove)
                ? CommandState.Completed
                : CommandState.Abandoned;
            
            args.CommandExecutor.QueueCommand(new CheckScenarioWonCommand());

            return UniTask.FromResult(commandState);
        }
    }
}