using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public class MoveChessBoardObjectCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly Vector2Int chessBoardObjectStartingPosition;
        private readonly Vector2Int chessBoardObjectEndingPosition;
        
        public MoveChessBoardObjectCommand(Vector2Int start, Vector2Int end)
        {
            chessBoardObjectStartingPosition = start;
            chessBoardObjectEndingPosition = end;
        }
        
        public CommandState Execute(ChessBoardCommandArgs args)
        {
            ChessBoardObjectInstance chessBoardObjectInstance =
                args.chessBoard.GetChessBoardObjectAt(chessBoardObjectStartingPosition);

            if (chessBoardObjectInstance != null)
            {
                if (chessBoardObjectInstance.TryFindComponent<IGrid2DTransform>(out var grid2DTransform))
                {
                    grid2DTransform.iFace.SetPosition(grid2DTransform.instance, chessBoardObjectEndingPosition);
                }
            }
            else
            {
                UnityEngine.Debug.LogAssertion($"Failed to find chess board object at position {chessBoardObjectStartingPosition} when executing move command.");
            }

            return CommandState.Finished;
        }
    }
}