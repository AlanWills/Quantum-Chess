using Cysharp.Threading.Tasks;

namespace QC.Match.Commands
{
    public readonly struct EnableInputForChessPieceColourCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly ChessPieceColour colour;

        public EnableInputForChessPieceColourCommand(ChessPieceColour colour)
        {
            this.colour = colour;
        }
        
        public async UniTask<CommandState> Execute(ChessBoardCommandArgs args)
        {
            var commandState = await InputCommandUtilities.SetInteractableForColour(args.ChessBoard, colour, true);
            
            if (commandState == CommandState.Completed)
            {
                // Clear the selection when changing what pieces are interactable
                args.CommandExecutor.QueueCommand(new ClearSelectedChessBoardObjectCommand());
            }
            
            return commandState;
        }
    }
}