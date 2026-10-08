using System.Threading;
using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public readonly struct MoveBoardObjectFromToCommand : ICommand<BoardCommandArgs>
    {
        private readonly Vector2Int boardObjectStartingPosition;
        private readonly Vector2Int boardObjectEndingPosition;
        
        public MoveBoardObjectFromToCommand(Vector2Int start, Vector2Int end)
        {
            boardObjectStartingPosition = start;
            boardObjectEndingPosition = end;
        }
        
        public UniTask<CommandState> Execute(BoardCommandArgs args, CancellationToken token)
        {
            BoardObjectInstance boardObjectInstance =
                args.Board.GetBoardObjectAt(boardObjectStartingPosition);

            return MoveCommandUtilities.TryMoveBoardObject(
                args, 
                boardObjectInstance, 
                boardObjectEndingPosition);
        }

        public override string ToString()
        {
            return $"{nameof(MoveBoardObjectFromToCommand)} - Starting Position: {boardObjectStartingPosition} - Ending Position: {boardObjectEndingPosition}";
        }
    }
}