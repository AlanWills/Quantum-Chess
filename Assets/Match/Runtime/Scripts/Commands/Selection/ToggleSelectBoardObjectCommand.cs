using System.Threading;
using Cysharp.Threading.Tasks;
using QC.Match.Objects;

namespace QC.Match.Commands
{
    public readonly struct ToggleSelectBoardObjectCommand : ICommand<BoardCommandArgs>
    {
        private readonly BoardObjectInstance boardObjectInstance;
        
        public ToggleSelectBoardObjectCommand(BoardObjectInstance instance)
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
                return await args.CommandExecutor.ExecuteCommandImmediately(new ClearSelectedBoardObjectCommand(), args);
            }
            else
            {
                return await args.CommandExecutor.ExecuteCommandImmediately(new SelectBoardObjectCommand(boardObjectInstance), args);
            }
        }

        public override string ToString()
        {
            return $"{nameof(ToggleSelectBoardObjectCommand)} - Instance Id: {boardObjectInstance.InstanceId}";
        }
    }
}