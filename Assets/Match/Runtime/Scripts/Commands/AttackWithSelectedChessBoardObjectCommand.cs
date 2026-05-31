using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public readonly struct AttackWithSelectedChessBoardObjectCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly Vector2Int chessBoardObjectEndingPosition;
        
        public AttackWithSelectedChessBoardObjectCommand(Vector2Int end)
        {
            chessBoardObjectEndingPosition = end;
        }
        
        public CommandState Execute(ChessBoardCommandArgs args)
        {
            ChessBoardObjectInstance chessBoardObjectInstance = args.Selection.CurrentSelectedObject;
            return AttackCommandUtils.TryAttackWithChessBoardObject(
                args, 
                chessBoardObjectInstance,
                chessBoardObjectEndingPosition);
        }
    }
}