using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public static class MoveCommandUtils
    {
        public static CommandState TryMoveChessBoardObject(
            ChessBoardCommandArgs args,
            ChessBoardObjectInstance instance,
            Vector2Int endingPosition)
        {
            if (instance == null)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to move a null chess board object when executing move command.");
                return CommandState.Abandoned;
            }
            
            if (!instance.TryFindComponent<IGrid2DTransform>(out var grid2DTransform))
            {
                UnityEngine.Debug.LogAssertion("Could not find transform component for chess board object when executing move object command.");
                return CommandState.Abandoned;
            }
            
            ChessBoardObjectInstance endingPositionInstance = args.ChessBoard.GetChessBoardObjectAt(endingPosition);
            
            if (endingPositionInstance != null)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to move chess board object to {endingPosition} which already has a piece there.");
                return CommandState.Abandoned;
            }
            
            grid2DTransform.iFace.SetPosition(grid2DTransform.instance, endingPosition);
            
            // Clear selection on successful move
            args.Selection.CurrentSelectedObject = null;
            
            return CommandState.Completed;
        }
    }
}