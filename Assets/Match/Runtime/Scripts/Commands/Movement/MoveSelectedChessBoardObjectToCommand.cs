using System.Threading;
using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public readonly struct MoveSelectedChessBoardObjectToCommand : ICommand<BoardCommandArgs>
    {
        private readonly Vector2Int chessBoardObjectEndingPosition;
        
        public MoveSelectedChessBoardObjectToCommand(Vector2Int end)
        {
            chessBoardObjectEndingPosition = end;
        }
        
        public UniTask<CommandState> Execute(BoardCommandArgs args, CancellationToken token)
        {
            BoardObjectInstance boardObjectInstance = args.Selection.CurrentSelectedObject;
            return MoveCommandUtilities.TryMoveChessBoardObject(
                args, 
                boardObjectInstance,
                chessBoardObjectEndingPosition);
        }
    }
}