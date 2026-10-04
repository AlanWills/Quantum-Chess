using System.Threading;
using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public readonly struct AttackWithChessBoardObjectFromToCommand : ICommand<BoardCommandArgs>
    {
        private readonly Vector2Int chessBoardObjectStartingPosition;
        private readonly Vector2Int chessBoardObjectEndingPosition;
        
        public AttackWithChessBoardObjectFromToCommand(Vector2Int start, Vector2Int end)
        {
            chessBoardObjectStartingPosition = start;
            chessBoardObjectEndingPosition = end;
        }
        
        public UniTask<CommandState> Execute(BoardCommandArgs args, CancellationToken token)
        {
            BoardObjectInstance boardObjectInstance =
                args.Board.GetBoardObjectAt(chessBoardObjectStartingPosition);

            return AttackCommandUtilities.TryAttackWithChessBoardObject(
                args, 
                boardObjectInstance, 
                chessBoardObjectEndingPosition);
        }

        public override string ToString()
        {
            return $"{nameof(AttackWithChessBoardObjectFromToCommand)} - Starting Position: {chessBoardObjectStartingPosition} - Ending Position: {chessBoardObjectEndingPosition}";
        }
    }
}