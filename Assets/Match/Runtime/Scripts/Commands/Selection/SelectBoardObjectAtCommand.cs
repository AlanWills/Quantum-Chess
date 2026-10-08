using System.Threading;
using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public readonly struct SelectBoardObjectAtCommand : ICommand<BoardCommandArgs>
    {
        private readonly Vector2Int chessBoardObjectLocation;
        
        public SelectBoardObjectAtCommand(Vector2Int location)
        {
            chessBoardObjectLocation = location;
        }
        
        public UniTask<CommandState> Execute(BoardCommandArgs args, CancellationToken token)
        {
            BoardObjectInstance boardObjectInstance = args.Board.GetBoardObjectAt(chessBoardObjectLocation);
            if (boardObjectInstance == null)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to select a {nameof(BoardObjectInstance)} at {chessBoardObjectLocation}, but there is no board object there.  Did you mean to clear selection?");
                return UniTask.FromResult(CommandState.Completed);
            }
            
            args.Selection.CurrentSelectedObject = boardObjectInstance;
            return UniTask.FromResult(CommandState.Completed);
        }

        public override string ToString()
        {
            return $"{nameof(SelectBoardObjectAtCommand)} - Location: {chessBoardObjectLocation}";
        }
    }
}