using System.Threading;
using Cysharp.Threading.Tasks;
using QC.Match.Objects;

namespace QC.Match.Commands
{
    public readonly struct ToggleSelectChessBoardObjectCommand : ICommand<BoardCommandArgs>
    {
        private readonly BoardObjectInstance boardObjectInstance;
        
        public ToggleSelectChessBoardObjectCommand(BoardObjectInstance instance)
        {
            boardObjectInstance = instance;
        }
        
        public async UniTask<CommandState> Execute(BoardCommandArgs args, CancellationToken token)
        {
            if (boardObjectInstance == null)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to toggle the selection of a {nameof(BoardObjectInstance)} that is null.");
                return CommandState.Completed;
            }

            if (args.Selection.CurrentSelectedObject == boardObjectInstance)
            {
                return await args.CommandExecutor.ExecuteCommandImmediately(new ClearSelectedChessBoardObjectCommand(), args);
            }
            else
            {
                return await args.CommandExecutor.ExecuteCommandImmediately(new SelectChessBoardObjectCommand(boardObjectInstance), args);
            }
        }
    }
}