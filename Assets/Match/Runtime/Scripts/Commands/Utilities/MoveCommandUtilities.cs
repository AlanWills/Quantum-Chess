using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public static class MoveCommandUtilities
    {
        public static UniTask<CommandState> TryMoveChessBoardObject(
            ChessBoardCommandArgs args,
            ChessBoardObjectInstance instance,
            Vector2Int endingPosition)
        {
            if (instance == null)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to move a null chess board object when executing move command.");
                return UniTask.FromResult(CommandState.Abandoned);
            }
            
            if (!instance.TryFindComponent<IGrid2DTransform>(out var grid2DTransform))
            {
                UnityEngine.Debug.LogAssertion("Could not find transform component for chess board object when executing move object command.");
                return UniTask.FromResult(CommandState.Abandoned);
            }
            
            ChessBoardObjectInstance endingPositionInstance = args.ChessBoard.GetChessBoardObjectAt(endingPosition);
            
            if (endingPositionInstance != null)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to move chess board object to {endingPosition} which already has a piece there.");
                return UniTask.FromResult(CommandState.Abandoned);
            }
            
            grid2DTransform.iFace.SetPosition(grid2DTransform.instance, endingPosition);
            
            if (instance.TryFindComponent<IObjectStatus>(out var status))
            {
                status.iFace.SetHasMoved(status.instance, true);
            }
            
            args.CommandExecutor.QueueCommand(new ClearSelectedChessBoardObjectCommand());
            
            // This could be a problem in the future if we allow multiple moves
            args.CommandExecutor.QueueCommand(new EndTurnCommand());

            return UniTask.FromResult(CommandState.Completed);
        }
    }
}