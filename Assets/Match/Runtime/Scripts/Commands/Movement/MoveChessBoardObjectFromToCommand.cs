using System.Threading;
using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public readonly struct MoveChessBoardObjectFromToCommand : ICommand<BoardCommandArgs>
    {
        private readonly Vector2Int chessBoardObjectStartingPosition;
        private readonly Vector2Int chessBoardObjectEndingPosition;
        
        public MoveChessBoardObjectFromToCommand(Vector2Int start, Vector2Int end)
        {
            chessBoardObjectStartingPosition = start;
            chessBoardObjectEndingPosition = end;
        }
        
        public UniTask<CommandState> Execute(BoardCommandArgs args, CancellationToken token)
        {
            BoardObjectInstance boardObjectInstance =
                args.Board.GetBoardObjectAt(chessBoardObjectStartingPosition);

            return MoveCommandUtilities.TryMoveChessBoardObject(
                args, 
                boardObjectInstance, 
                chessBoardObjectEndingPosition);
        }

        public override string ToString()
        {
            return $"{nameof(MoveChessBoardObjectFromToCommand)} - Starting Position: {chessBoardObjectStartingPosition} - Ending Position: {chessBoardObjectEndingPosition}";
        }
    }
}