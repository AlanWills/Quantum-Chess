using System.Threading;
using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public readonly struct RemoveChessBoardObjectAtCommand : ICommand<BoardCommandArgs>
    {
        private readonly Vector2Int BoardPosition;

        public RemoveChessBoardObjectAtCommand(Vector2Int boardPosition)
        {
            BoardPosition = boardPosition;
        }

        public UniTask<CommandState> Execute(BoardCommandArgs args, CancellationToken token)
        {
            BoardObjectInstance instance = args.Board.GetBoardObjectAt(BoardPosition);
            if (instance == null)
            {
                UnityEngine.Debug.LogAssertion($"Could not find chess board object at position {BoardPosition}.");
                return UniTask.FromResult(CommandState.Abandoned);
            }
            
            return RemoveCommandUtilities.RemoveChessBoardObject(args, instance);
        }

        public override string ToString()
        {
            return $"{nameof(RemoveChessBoardObjectAtCommand)} - Position: {BoardPosition}";
        }
    }
}