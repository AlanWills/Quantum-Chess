using System.Threading;
using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public readonly struct AttackWithSelectedChessBoardObjectCommand : ICommand<BoardCommandArgs>
    {
        private readonly Vector2Int chessBoardObjectEndingPosition;
        
        public AttackWithSelectedChessBoardObjectCommand(Vector2Int end)
        {
            chessBoardObjectEndingPosition = end;
        }
        
        public UniTask<CommandState> Execute(BoardCommandArgs args, CancellationToken token)
        {
            BoardObjectInstance boardObjectInstance = args.Selection.CurrentSelectedObject;
            return AttackCommandUtilities.TryAttackWithChessBoardObject(
                args, 
                boardObjectInstance,
                chessBoardObjectEndingPosition);
        }

        public override string ToString()
        {
            return $"{nameof(AttackWithSelectedChessBoardObjectCommand)} - Ending Position: {chessBoardObjectEndingPosition}";
        }
    }
}