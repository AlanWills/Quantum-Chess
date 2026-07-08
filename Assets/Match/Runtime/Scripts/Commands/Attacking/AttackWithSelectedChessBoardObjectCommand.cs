using System.Threading;
using Cysharp.Threading.Tasks;
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
        
        public UniTask<CommandState> Execute(ChessBoardCommandArgs args, CancellationToken token)
        {
            ChessBoardObjectInstance chessBoardObjectInstance = args.Selection.CurrentSelectedObject;
            return AttackCommandUtilities.TryAttackWithChessBoardObject(
                args, 
                chessBoardObjectInstance,
                chessBoardObjectEndingPosition);
        }
    }
}