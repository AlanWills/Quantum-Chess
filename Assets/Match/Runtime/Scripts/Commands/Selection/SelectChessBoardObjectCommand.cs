using System.Threading;
using Cysharp.Threading.Tasks;
using QC.Match.Objects;

namespace QC.Match.Commands
{
    public readonly struct SelectChessBoardObjectCommand : ICommand<BoardCommandArgs>
    {
        private readonly BoardObjectInstance boardObjectInstance;
        
        public SelectChessBoardObjectCommand(BoardObjectInstance instance)
        {
            boardObjectInstance = instance;
        }
        
        public UniTask<CommandState> Execute(BoardCommandArgs args, CancellationToken token)
        {
            if (boardObjectInstance == null)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to select a {nameof(BoardObjectInstance)} that is null.");
                return UniTask.FromResult(CommandState.Completed);
            }
            
            args.Selection.CurrentSelectedObject = boardObjectInstance;
            return UniTask.FromResult(CommandState.Completed);
        }

        public override string ToString()
        {
            return $"{nameof(SelectChessBoardObjectCommand)} - Instance ID: {boardObjectInstance.InstanceId}";
        }
    }
}