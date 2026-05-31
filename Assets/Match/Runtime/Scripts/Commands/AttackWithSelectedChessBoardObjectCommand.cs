using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public readonly struct MoveSelectedChessBoardObjectToCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly Vector2Int chessBoardObjectEndingPosition;
        
        public MoveSelectedChessBoardObjectToCommand(Vector2Int end)
        {
            chessBoardObjectEndingPosition = end;
        }
        
        public CommandState Execute(ChessBoardCommandArgs args)
        {
            ChessBoardObjectInstance chessBoardObjectInstance = args.Selection.CurrentSelectedObject;
            
            CommandState commandState = MoveCommandUtils.TryMoveChessBoardObject(
                args, 
                chessBoardObjectInstance,
                chessBoardObjectEndingPosition);

            if (commandState == CommandState.Completed)
            {
                // Clear selection on successful move
                args.Selection.CurrentSelectedObject = null;
            }

            return commandState;
        }
    }
}