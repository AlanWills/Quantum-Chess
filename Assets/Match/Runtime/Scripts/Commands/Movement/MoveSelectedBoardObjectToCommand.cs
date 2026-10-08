using System.Threading;
using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public readonly struct MoveSelectedBoardObjectToCommand : ICommand<BoardCommandArgs>
    {
        private readonly Vector2Int boardObjectEndingPosition;
        
        public MoveSelectedBoardObjectToCommand(Vector2Int end)
        {
            boardObjectEndingPosition = end;
        }
        
        public UniTask<CommandState> Execute(BoardCommandArgs args, CancellationToken token)
        {
            BoardObjectInstance boardObjectInstance = args.Selection.CurrentSelectedObject;
            return MoveCommandUtilities.TryMoveBoardObject(
                args, 
                boardObjectInstance,
                boardObjectEndingPosition);
        }

        public override string ToString()
        {
            return $"{nameof(MoveSelectedBoardObjectToCommand)} - Ending Position: {boardObjectEndingPosition}";
        }
    }
}