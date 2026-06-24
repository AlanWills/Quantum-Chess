using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public static class MoveCommandUtilities
    {
        public static async UniTask<CommandState> TryMoveChessBoardObject(
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
            
            if (instance.TryFindComponent<IObjectStatus>(out var status))
            {
                status.iFace.SetHasMoved(status.instance, true);
            }

            CommandState clearSelectedChessBoardObjectResult = await args.CommandExecutor.ExecuteCommandImmediately(new ClearSelectedChessBoardObjectCommand(), args);
            UnityEngine.Debug.Assert(clearSelectedChessBoardObjectResult == CommandState.Completed, $"There was an issue clearing the selected chess board object after trying to move it.");
            
            // This could be a problem in the future if we allow multiple moves
            // Queue this up, as there may be other things that need resolving before we switch players
            args.CommandExecutor.QueueCommand(new SetActivePlayerCommand(args.MatchState.NextPlayerIndex));

            return CommandState.Completed;
        }
    }
}