using System.Threading;
using Cysharp.Threading.Tasks;
using Lua;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public readonly struct AddBoardTileCommand : ICommand<BoardCommandArgs>
    {
        private readonly int Guid;
        private readonly Vector2Int Position;
        private readonly int Importance;

        public AddBoardTileCommand(
            int guid,
            Vector2Int position,
            int importance)
        {
            Guid = guid;
            Position = position;
            Importance = importance;
        }
        
        public UniTask<CommandState> Execute(BoardCommandArgs args, CancellationToken token)
        {
            BoardTileInstance instance = args.Board.CreateBoardTile(Guid, Position);
            if (instance == null)
            {
                UnityEngine.Debug.LogError($"Failed to create chess board tile with guid {Guid} at position {Position}.");
                return UniTask.FromResult(CommandState.Abandoned);
            }

            if (instance.TryFindInterface<ITileAIInfo>(out var tileAIInfo))
            {
                tileAIInfo.SetImportance(Importance);
            }

            args.Board.AddBoardTileInstance(instance);

            return UniTask.FromResult(CommandState.Completed);
        }

        public override string ToString()
        {
            return $"{nameof(AddBoardTileCommand)} - Guid: {Guid} - Position: {Position} - Importance: {Importance}";
        }
    }
}