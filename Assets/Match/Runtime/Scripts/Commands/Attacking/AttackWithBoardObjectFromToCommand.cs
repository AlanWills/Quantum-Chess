using System.Threading;
using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public readonly struct AttackWithBoardObjectFromToCommand : ICommand<BoardCommandArgs>
    {
        private readonly Vector2Int boardObjectStartingPosition;
        private readonly Vector2Int boardObjectEndingPosition;
        
        public AttackWithBoardObjectFromToCommand(Vector2Int start, Vector2Int end)
        {
            boardObjectStartingPosition = start;
            boardObjectEndingPosition = end;
        }
        
        public UniTask<CommandState> Execute(BoardCommandArgs args, CancellationToken token)
        {
            BoardObjectInstance boardObjectInstance =
                args.Board.GetBoardObjectAt(boardObjectStartingPosition);

            return AttackCommandUtilities.TryAttackWithBoardObject(
                args, 
                boardObjectInstance, 
                boardObjectEndingPosition);
        }

        public override string ToString()
        {
            return $"{nameof(AttackWithBoardObjectFromToCommand)} - Starting Position: {boardObjectStartingPosition} - Ending Position: {boardObjectEndingPosition}";
        }
    }
}