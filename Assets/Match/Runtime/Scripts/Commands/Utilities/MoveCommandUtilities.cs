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
            
            CommandState clearSelectedChessBoardObjectResult = await args.CommandExecutor.ExecuteCommandImmediately(new ClearSelectedChessBoardObjectCommand(), args);
            UnityEngine.Debug.Assert(clearSelectedChessBoardObjectResult == CommandState.Completed, $"There was an issue clearing the selected chess board object after trying to move it.");

            // Animations will be changing Unity Objects, so it's best to by default switch to the main thread
            // This will avoid any issues changing transforms on other threads
            await UniTask.SwitchToMainThread();
            await args.VisualAnimationSystem.AnimateTo(instance, endingPosition);
            
            grid2DTransform.SetPosition(endingPosition);
            
            await UniTask.SwitchToThreadPool();

            if (instance.TryFindComponent<IObjectStatus>(out var status))
            {
                status.SetHasMoved(true);
            }

            // This could be a problem in the future if we allow multiple moves
            // Queue this up, as there may be other things that need resolving before we switch players
            args.CommandExecutor.QueueCommand(new SetActivePlayerCommand(args.MatchState.NextPlayerIndex));

            return CommandState.Completed;
        }
    }
}