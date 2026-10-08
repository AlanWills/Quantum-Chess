using Cysharp.Threading.Tasks;
using QC.Match.Objects;

namespace QC.Match.Commands
{
    public static class RemoveCommandUtilities
    {
        public static UniTask<CommandState> RemoveBoardObject(
            BoardCommandArgs args,
            BoardObjectInstance objectToRemove)
        {
            CommandState commandState = args.Board.RemoveBoardObject(objectToRemove)
                ? CommandState.Completed
                : CommandState.Abandoned;

            return UniTask.FromResult(commandState);
        }
    }
}