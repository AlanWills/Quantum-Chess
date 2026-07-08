using System.Threading;
using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public readonly struct SelectChessBoardObjectAtCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly Vector2Int chessBoardObjectLocation;
        
        public SelectChessBoardObjectAtCommand(Vector2Int location)
        {
            chessBoardObjectLocation = location;
        }
        
        public UniTask<CommandState> Execute(ChessBoardCommandArgs args, CancellationToken token)
        {
            ChessBoardObjectInstance chessBoardObjectInstance = args.ChessBoard.GetChessBoardObjectAt(chessBoardObjectLocation);
            if (chessBoardObjectInstance == null)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to select a {nameof(ChessBoardObjectInstance)} at {chessBoardObjectLocation}, but there is no board object there.  Did you mean to clear selection?");
                return UniTask.FromResult(CommandState.Completed);
            }
            
            args.Selection.CurrentSelectedObject = chessBoardObjectInstance;
            return UniTask.FromResult(CommandState.Completed);
        }
    }
}