using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public readonly struct MoveChessBoardObjectFromToCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly Vector2Int chessBoardObjectStartingPosition;
        private readonly Vector2Int chessBoardObjectEndingPosition;
        
        public MoveChessBoardObjectFromToCommand(Vector2Int start, Vector2Int end)
        {
            chessBoardObjectStartingPosition = start;
            chessBoardObjectEndingPosition = end;
        }
        
        public UniTask<CommandState> Execute(ChessBoardCommandArgs args)
        {
            ChessBoardObjectInstance chessBoardObjectInstance =
                args.ChessBoard.GetChessBoardObjectAt(chessBoardObjectStartingPosition);

            return MoveCommandUtilities.TryMoveChessBoardObject(
                args, 
                chessBoardObjectInstance, 
                chessBoardObjectEndingPosition);
        }
    }
}