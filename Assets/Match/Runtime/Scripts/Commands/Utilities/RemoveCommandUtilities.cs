using Cysharp.Threading.Tasks;
using QC.Match.Objects;

namespace QC.Match.Commands
{
    public static class RemoveCommandUtilities
    {
        public static UniTask<CommandState> RemoveChessBoardObject(
            BoardCommandArgs args,
            BoardObjectInstance objectToRemove)
        {
            CommandState commandState = args.Board.RemoveChessBoardObject(objectToRemove)
                ? CommandState.Completed
                : CommandState.Abandoned;

            return UniTask.FromResult(commandState);
        }
    }
}