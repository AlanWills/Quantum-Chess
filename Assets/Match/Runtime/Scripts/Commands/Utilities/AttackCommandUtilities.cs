using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public static class AttackCommandUtilities
    {
        public static async UniTask<CommandState> TryAttackWithChessBoardObject(
            ChessBoardCommandArgs args,
            ChessBoardObjectInstance instance,
            Vector2Int endingPosition)
        {
            if (instance == null)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to move a null chess board object when executing attack command.");
                return CommandState.Abandoned;
            }
            
            if (!instance.TryFindComponent<IChessPiece>(out var chessPiece))
            {
                UnityEngine.Debug.LogAssertion($"Could not find {nameof(IChessPiece)} component for chess board object when executing attack command.");
                return CommandState.Abandoned;
            }
            
            ChessBoardObjectInstance endingPositionInstance = args.ChessBoard.GetChessBoardObjectAt(endingPosition);
            
            if (endingPositionInstance == null)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to attack with chess board object at {endingPosition}, but there is no chess board object there.");
                return CommandState.Abandoned;
            }

            if (!endingPositionInstance.TryFindComponent<IChessPiece>(out var otherChessPiece))
            {
                UnityEngine.Debug.LogAssertion($"Attempting to attack with chess board object at {endingPosition}, but there is no chess board object there with an {nameof(IChessPiece)} interface.");
                return CommandState.Abandoned;
            }

            ChessPieceColour chessPieceColour = chessPiece.GetPieceColour();
            if (chessPieceColour == otherChessPiece.GetPieceColour())
            {
                UnityEngine.Debug.LogAssertion($"Attempting to attack with chess board object at {endingPosition}, but there is a chess board object there with the same colour ({chessPieceColour}).");
                return CommandState.Abandoned;
            }

            if (await args.CommandExecutor.ExecuteCommandImmediately(new MoveChessBoardObjectToCommand(instance, endingPosition), args) == CommandState.Abandoned)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to move chess board object to {endingPosition} as the result of an attack, but there was an issue trying to move it on the board.");
                return CommandState.Abandoned;
            }

            if (await args.CommandExecutor.ExecuteCommandImmediately(new RemoveChessBoardObjectCommand(endingPositionInstance), args) == CommandState.Abandoned)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to remove chess board object at {endingPosition} as the result of an attack, but there was an issue trying to remove it from the board.");
                return CommandState.Abandoned;
            }

            return CommandState.Completed;
        }
    }
}