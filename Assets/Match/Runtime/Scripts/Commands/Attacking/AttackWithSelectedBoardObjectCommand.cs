using System.Threading;
using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public readonly struct AttackWithSelectedBoardObjectCommand : ICommand<BoardCommandArgs>
    {
        private readonly Vector2Int boardObjectEndingPosition;
        
        public AttackWithSelectedBoardObjectCommand(Vector2Int end)
        {
            boardObjectEndingPosition = end;
        }
        
        public UniTask<CommandState> Execute(BoardCommandArgs args, CancellationToken token)
        {
            BoardObjectInstance boardObjectInstance = args.Selection.CurrentSelectedObject;
            return AttackCommandUtilities.TryAttackWithBoardObject(
                args, 
                boardObjectInstance,
                boardObjectEndingPosition);
        }

        public override string ToString()
        {
            return $"{nameof(AttackWithSelectedBoardObjectCommand)} - Ending Position: {boardObjectEndingPosition}";
        }
    }
}