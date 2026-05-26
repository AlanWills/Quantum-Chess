using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public readonly struct MoveSelectedChessBoardObjectFromToCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly Vector2Int chessBoardObjectEndingPosition;
        
        public MoveSelectedChessBoardObjectFromToCommand(Vector2Int end)
        {
            chessBoardObjectEndingPosition = end;
        }
        
        public CommandState Execute(ChessBoardCommandArgs args)
        {
            ChessBoardObjectInstance chessBoardObjectInstance = args.Selection.CurrentSelectedObject;
            
            if (chessBoardObjectInstance != null)
            {
                if (chessBoardObjectInstance.TryFindComponent<IGrid2DTransform>(out var grid2DTransform))
                {
                    grid2DTransform.iFace.SetPosition(grid2DTransform.instance, chessBoardObjectEndingPosition);
                }
            }
            else
            {
                UnityEngine.Debug.LogAssertion($"No selected chess board object when executing move selected object command.");
            }

            return CommandState.Finished;
        }
    }
}