using Cysharp.Threading.Tasks;
using Lua;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public readonly struct AddChessBoardTileCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly int Guid;
        private readonly Vector2Int Position;
        private readonly int Importance;

        public AddChessBoardTileCommand(
            int guid,
            Vector2Int position,
            int importance)
        {
            Guid = guid;
            Position = position;
            Importance = importance;
        }
        
        public UniTask<CommandState> Execute(ChessBoardCommandArgs args)
        {
            ChessBoardTileInstance instance = args.ChessBoard.CreateChessBoardTile(Guid, Position);
            if (instance == null)
            {
                UnityEngine.Debug.LogError($"Failed to create chess board tile with guid {Guid} at position {Position}.");
                return UniTask.FromResult(CommandState.Abandoned);
            }

            if (instance.TryFindComponent<ITileAIInfo>(out var tileAIInfo))
            {
                tileAIInfo.SetImportance(Importance);
            }

            args.ChessBoard.AddChessBoardTileInstance(instance);

            return UniTask.FromResult(CommandState.Completed);
        }
    }
}