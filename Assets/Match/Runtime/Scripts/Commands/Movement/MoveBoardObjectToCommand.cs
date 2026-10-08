using System.Threading;
using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public readonly struct MoveBoardObjectToCommand : ICommand<BoardCommandArgs>
    {
        private readonly BoardObjectInstance boardObjectInstance;
        private readonly Vector2Int boardObjectEndingPosition;
        
        public MoveBoardObjectToCommand(BoardObjectInstance instance, Vector2Int end)
        {
            boardObjectInstance = instance;
            boardObjectEndingPosition = end;
        }
        
        public UniTask<CommandState> Execute(BoardCommandArgs args, CancellationToken token)
        {
            return MoveCommandUtilities.TryMoveBoardObject(
                args, 
                boardObjectInstance, 
                boardObjectEndingPosition);
        }

        public override string ToString()
        {
            return $"{nameof(MoveBoardObjectToCommand)} - Instance Id: {boardObjectInstance.InstanceId} - Ending Position: {boardObjectEndingPosition}";
        }
    }
}