using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public readonly struct RemoveChessBoardObjectAtCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly Vector2Int BoardPosition;

        public RemoveChessBoardObjectAtCommand(Vector2Int boardPosition)
        {
            BoardPosition = boardPosition;
        }

        public CommandState Execute(ChessBoardCommandArgs args)
        {
            ChessBoardObjectInstance instance = args.ChessBoard.GetChessBoardObjectAt(BoardPosition);
            if (instance == null)
            {
                UnityEngine.Debug.LogAssertion($"Could not find chess board object at position {BoardPosition}.");
                return CommandState.Abandoned;
            }
            
            return args.ChessBoard.RemoveChessBoardObject(instance)
                ? CommandState.Completed
                : CommandState.Abandoned;
        }
    }
}