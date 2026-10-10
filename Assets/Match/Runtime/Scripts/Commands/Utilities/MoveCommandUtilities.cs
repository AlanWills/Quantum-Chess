using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public static class MoveCommandUtilities
    {
        public static async UniTask<CommandState> TryMoveBoardObject(
            BoardCommandArgs args,
            BoardObjectInstance instance,
            Vector2Int endingPosition)
        {
            if (instance == null)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to move a null board object when executing move command.");
                return CommandState.Abandoned;
            }

            if (!instance.TryFindInterface<IGrid2DTransform>(out var grid2DTransform))
            {
                UnityEngine.Debug.LogAssertion("Could not find transform component for board object when executing move object command.");
                return CommandState.Abandoned;
            }
            
            if (!args.Board.IsPositionInBoard(endingPosition))
            {
                UnityEngine.Debug.LogAssertion($"Ending Position for movement {endingPosition} is outside the bounding box of the board.");
                return CommandState.Abandoned;
            }
            
            CommandState clearSelectedChessBoardObjectResult = await args.CommandExecutor.ExecuteCommandImmediately(new ClearSelectedBoardObjectCommand(), args);
            UnityEngine.Debug.Assert(clearSelectedChessBoardObjectResult == CommandState.Completed, $"There was an issue clearing the selected board object after trying to move it.");

            await args.VisualAnimationSystem.AnimateTo(instance, endingPosition);
                
            grid2DTransform.SetPosition(endingPosition);

            if (args.PromotionSystem.CanPromote(instance, endingPosition, args.Board))
            {
                BoardObjectInstance newInstance = await args.PromotionSystem.Promote(instance, endingPosition, args.Board);
                UnityEngine.Debug.Assert(newInstance != null, $"{nameof(IPromotionSystem)} returned a null promoted piece, meaning something has gone wrong!");
                instance = newInstance != null ? newInstance : instance;
            }
            
            if (instance.TryFindInterface<IObjectStatus>(out var status))
            {
                status.SetHasMoved(true);
            }
            
            if (instance.TryFindInterface<IPiece>(out var chessPiece))
            {
                PieceColour pieceColour = chessPiece.GetPieceColour();
                int playerIndex = args.MatchState.GetPlayerIndex(pieceColour);
                args.MatchState.RecordActionTakenByPlayer(playerIndex);
            }

            return CommandState.Completed;
        }
    }
}