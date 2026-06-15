using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public readonly struct MoveChessBoardObjectToCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly ChessBoardObjectInstance chessBoardObjectInstance;
        private readonly Vector2Int chessBoardObjectEndingPosition;
        
        public MoveChessBoardObjectToCommand(ChessBoardObjectInstance instance, Vector2Int end)
        {
            chessBoardObjectInstance = instance;
            chessBoardObjectEndingPosition = end;
        }
        
        public UniTask<CommandState> Execute(ChessBoardCommandArgs args)
        {
            return MoveCommandUtilities.TryMoveChessBoardObject(
                args, 
                chessBoardObjectInstance, 
                chessBoardObjectEndingPosition);
        }
    }
}