using System.Threading;
using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public readonly struct MoveChessBoardObjectToCommand : ICommand<BoardCommandArgs>
    {
        private readonly BoardObjectInstance boardObjectInstance;
        private readonly Vector2Int chessBoardObjectEndingPosition;
        
        public MoveChessBoardObjectToCommand(BoardObjectInstance instance, Vector2Int end)
        {
            boardObjectInstance = instance;
            chessBoardObjectEndingPosition = end;
        }
        
        public UniTask<CommandState> Execute(BoardCommandArgs args, CancellationToken token)
        {
            return MoveCommandUtilities.TryMoveChessBoardObject(
                args, 
                boardObjectInstance, 
                chessBoardObjectEndingPosition);
        }

        public override string ToString()
        {
            return $"{nameof(MoveChessBoardObjectToCommand)} - Instance Id: {boardObjectInstance.InstanceId} - Ending Position: {chessBoardObjectEndingPosition}";
        }
    }
}