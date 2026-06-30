using Cysharp.Threading.Tasks;
using QC.Match.Objects;

namespace QC.Match.Commands
{
    public readonly struct ToggleSelectChessBoardObjectCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly ChessBoardObjectInstance chessBoardObjectInstance;
        
        public ToggleSelectChessBoardObjectCommand(ChessBoardObjectInstance instance)
        {
            chessBoardObjectInstance = instance;
        }
        
        public async UniTask<CommandState> Execute(ChessBoardCommandArgs args)
        {
            if (chessBoardObjectInstance == null)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to toggle the selection of a {nameof(ChessBoardObjectInstance)} that is null.");
                return CommandState.Completed;
            }

            if (args.Selection.CurrentSelectedObject == chessBoardObjectInstance)
            {
                return await args.CommandExecutor.ExecuteCommandImmediately(new ClearSelectedChessBoardObjectCommand(), args);
            }
            else
            {
                return await args.CommandExecutor.ExecuteCommandImmediately(new SelectChessBoardObjectCommand(chessBoardObjectInstance), args);
            }
        }
    }
}