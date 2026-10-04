using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public static class MoveCommandUtilities
    {
        public static async UniTask<CommandState> TryMoveChessBoardObject(
            BoardCommandArgs args,
            BoardObjectInstance instance,
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
            
            RectInt boundingBox = args.Board.CalculateBoundingBox();
            if (!boundingBox.Contains(endingPosition))
            {
                UnityEngine.Debug.LogAssertion($"Ending Position for movement {endingPosition} is outside the bounding box of the board {boundingBox}.");
                return CommandState.Abandoned;
            }
            
            CommandState clearSelectedChessBoardObjectResult = await args.CommandExecutor.ExecuteCommandImmediately(new ClearSelectedChessBoardObjectCommand(), args);
            UnityEngine.Debug.Assert(clearSelectedChessBoardObjectResult == CommandState.Completed, $"There was an issue clearing the selected chess board object after trying to move it.");

            await args.VisualAnimationSystem.AnimateTo(instance, endingPosition);
            
            grid2DTransform.SetPosition(endingPosition);
            
            if (instance.TryFindComponent<IObjectStatus>(out var status))
            {
                status.SetHasMoved(true);
            }
            
            if (instance.TryFindComponent<IChessPiece>(out var chessPiece))
            {
                ChessPieceColour pieceColour = chessPiece.GetPieceColour();
                int playerIndex = args.MatchState.GetPlayerIndex(pieceColour);
                args.MatchState.RecordActionTakenByPlayer(playerIndex);
            }

            return CommandState.Completed;
        }
    }
}