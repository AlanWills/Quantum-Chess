using System.Collections.Generic;
using Celeste.DataStructures;
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
            
            if (args.ChessBoard.GetChessBoardObjectAt(endingPosition) != null)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to move chess board object to {endingPosition} which already has a piece there.");
                return CommandState.Abandoned;
            }

            if (!instance.TryFindComponent<IGrid2DTransform>(out var grid2DTransform))
            {
                UnityEngine.Debug.LogAssertion("Could not find transform component for chess board object when executing move object command.");
                return CommandState.Abandoned;
            }
            
            CommandState clearSelectedChessBoardObjectResult = await args.CommandExecutor.ExecuteCommandImmediately(new ClearSelectedChessBoardObjectCommand(), args);
            UnityEngine.Debug.Assert(clearSelectedChessBoardObjectResult == CommandState.Completed, $"There was an issue clearing the selected chess board object after trying to move it.");

            Vector2Int startingPosition = grid2DTransform.iFace.GetPosition(grid2DTransform.instance);
            List<Vector2Int> path = CalculatePathBetween(startingPosition, endingPosition, args.ChessBoard, instance); 
            float timePerSquare = 0.5f;
            float currentTimeElapsed = 0;
            int currentWaypointIndex = 0;

            while (currentWaypointIndex < path.Count - 1)
            {
                // While animating along our path
                if (currentTimeElapsed >= timePerSquare)
                {
                    // We should now move to the next waypoint 
                    currentTimeElapsed -= timePerSquare;
                    ++currentWaypointIndex;
                }

                currentTimeElapsed += args.TimeProvider.DeltaTime;
                grid2DTransform.iFace.SetPosition(grid2DTransform.instance, path.Get(currentWaypointIndex));

                await UniTask.Yield();
            }

            if (instance.TryFindComponent<IObjectStatus>(out var status))
            {
                status.iFace.SetHasMoved(status.instance, true);
            }

            // This could be a problem in the future if we allow multiple moves
            // Queue this up, as there may be other things that need resolving before we switch players
            args.CommandExecutor.QueueCommand(new SetActivePlayerCommand(args.MatchState.NextPlayerIndex));

            return CommandState.Completed;
        }

        private static List<Vector2Int> CalculatePathBetween(
            Vector2Int startingPosition,
            Vector2Int endingPosition,
            IChessBoardLookup chessBoardLookup,
            ChessBoardObjectInstance chessBoardObjectInstance)
        {
            // TODO: Need to calculate appropriate path to ending position, for now we'll just approximate using euclidean movement
            List<Vector2Int> path = new List<Vector2Int>();
            int yDelta = endingPosition.y - startingPosition.y;
            int ySign = yDelta > 0 ? 1 : -1;
            int xDelta = endingPosition.x - startingPosition.x;
            int xSign = xDelta > 0 ? 1 : -1;

            for (int y = 1; y <= Mathf.Abs(yDelta); ++y)
            {
                path.Add(new Vector2Int(startingPosition.x, startingPosition.y + y * ySign));
            }
            
            for (int x = 1; x <= Mathf.Abs(xDelta); ++x)
            {
                path.Add(new Vector2Int(startingPosition.x + x * xSign, endingPosition.y));
            }

            return path;
        }
    }
}