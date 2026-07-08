using System.Threading;
using Cysharp.Threading.Tasks;
using QC.Match.Objects;

namespace QC.Match.Commands
{
    public readonly struct SelectChessBoardObjectCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly ChessBoardObjectInstance chessBoardObjectInstance;
        
        public SelectChessBoardObjectCommand(ChessBoardObjectInstance instance)
        {
            chessBoardObjectInstance = instance;
        }
        
        public UniTask<CommandState> Execute(ChessBoardCommandArgs args, CancellationToken token)
        {
            if (chessBoardObjectInstance == null)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to select a {nameof(ChessBoardObjectInstance)} that is null.");
                return UniTask.FromResult(CommandState.Completed);
            }
            
            args.Selection.CurrentSelectedObject = chessBoardObjectInstance;
            return UniTask.FromResult(CommandState.Completed);
        }
    }
}