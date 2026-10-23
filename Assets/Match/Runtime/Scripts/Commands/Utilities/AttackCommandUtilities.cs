using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public static class AttackCommandUtilities
    {
        public static async UniTask<CommandState> TryAttackWithBoardObject(
            BoardCommandArgs args,
            BoardObjectInstance instance,
            Vector2Int endingPosition)
        {
            if (instance == null)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to move a null board object when executing attack command.");
                return CommandState.Abandoned;
            }
            
            if (!instance.Piece.IsValid)
            {
                UnityEngine.Debug.LogAssertion($"Could not find {nameof(IPiece)} component for board object when executing attack command.");
                return CommandState.Abandoned;
            }
            
            BoardObjectInstance endingPositionInstance = args.Board.GetBoardObjectAt(endingPosition);
            
            if (endingPositionInstance == null)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to attack with board object at {endingPosition}, but there is no board object there.");
                return CommandState.Abandoned;
            }

            if (!endingPositionInstance.Piece.IsValid)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to attack with board object at {endingPosition}, but there is no board object there with an {nameof(IPiece)} interface.");
                return CommandState.Abandoned;
            }

            PieceColour pieceColour = instance.Piece.GetPieceColour();
            if (pieceColour == endingPositionInstance.Piece.GetPieceColour())
            {
                UnityEngine.Debug.LogAssertion($"Attempting to attack with board object at {endingPosition}, but there is a board object there with the same colour ({pieceColour}).");
                return CommandState.Abandoned;
            }

            if (await args.CommandExecutor.ExecuteCommandImmediately(new MoveBoardObjectToCommand(instance, endingPosition), args) == CommandState.Abandoned)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to move board object to {endingPosition} as the result of an attack, but there was an issue trying to move it on the board.");
                return CommandState.Abandoned;
            }

            if (await args.CommandExecutor.ExecuteCommandImmediately(new RemoveBoardObjectCommand(endingPositionInstance), args) == CommandState.Abandoned)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to remove board object at {endingPosition} as the result of an attack, but there was an issue trying to remove it from the board.");
                return CommandState.Abandoned;
            }

            return CommandState.Completed;
        }
    }
}