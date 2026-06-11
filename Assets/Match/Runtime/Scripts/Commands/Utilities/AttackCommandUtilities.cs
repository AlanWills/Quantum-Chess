using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public static class AttackCommandUtilities
    {
        public static UniTask<CommandState> TryAttackWithChessBoardObject(
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
            
            if (!instance.TryFindComponent<IChessPiece>(out var chessPiece))
            {
                UnityEngine.Debug.LogAssertion($"Could not find {nameof(IChessPiece)} component for chess board object when executing move object command.");
                return UniTask.FromResult(CommandState.Abandoned);
            }
            
            ChessBoardObjectInstance endingPositionInstance = args.ChessBoard.GetChessBoardObjectAt(endingPosition);
            
            if (endingPositionInstance == null)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to attack with chess board object at {endingPosition}, but there is no chess board object there.");
                return UniTask.FromResult(CommandState.Abandoned);
            }

            if (!endingPositionInstance.TryFindComponent<IChessPiece>(out var otherChessPiece))
            {
                UnityEngine.Debug.LogAssertion($"Attempting to attack with chess board object at {endingPosition}, but there is no chess board object there with an {nameof(IChessPiece)} interface.");
                return UniTask.FromResult(CommandState.Abandoned);
            }

            ChessPieceColour chessPieceColour = chessPiece.iFace.GetPieceColour(chessPiece.instance);
            if (chessPieceColour == otherChessPiece.iFace.GetPieceColour(otherChessPiece.instance))
            {
                UnityEngine.Debug.LogAssertion($"Attempting to attack with chess board object at {endingPosition}, but there is a chess board object there with the same colour ({chessPieceColour}).");
                return UniTask.FromResult(CommandState.Abandoned);
            }

            grid2DTransform.iFace.SetPosition(grid2DTransform.instance, endingPosition);
            
            if (instance.TryFindComponent<IObjectStatus>(out var status))
            {
                status.iFace.SetHasMoved(status.instance, true);
            }
            
            args.CommandExecutor.QueueCommand(new ClearSelectedChessBoardObjectCommand());
            args.ChessBoard.RemoveChessBoardObject(endingPositionInstance);
            args.CommandExecutor.QueueCommand(new SetActiveChessPieceColourCommand(args.MatchState.NextActiveColour));

            return UniTask.FromResult(CommandState.Completed);
        }
    }
}