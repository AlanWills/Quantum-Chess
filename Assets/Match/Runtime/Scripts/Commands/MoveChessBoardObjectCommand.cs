using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public class MoveChessBoardObjectCommand : ICommand
    {
        private readonly ChessBoardObjectInstance chessBoardObjectInstance;
        public readonly Vector2Int targetPosition;
        
        public MoveChessBoardObjectCommand(ChessBoardObjectInstance instance, Vector2Int target)
        {
            chessBoardObjectInstance = instance;
            targetPosition = target;
        }
        
        public CommandState Execute()
        {
            if (chessBoardObjectInstance.TryFindComponent<IGrid2DTransform>(out var grid2DTransform))
            {
                grid2DTransform.iFace.SetPosition(grid2DTransform.instance, targetPosition);
            }

            return CommandState.Finished;
        }
    }
}