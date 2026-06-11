using Cysharp.Threading.Tasks;

namespace QC.Match.Commands
{
    public readonly struct DisableInputIfNotChessPieceColourCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly ChessPieceColour colour;

        public DisableInputIfNotChessPieceColourCommand(ChessPieceColour colour)
        {
            this.colour = colour;
        }
        
        public async UniTask<CommandState> Execute(ChessBoardCommandArgs args)
        {
            var commandState = await InputCommandUtilities.SetInteractableForNotColour(args.ChessBoard, colour, false);
            
            if (commandState == CommandState.Completed)
            {
                // Clear the selection when changing what pieces are interactable
                args.CommandExecutor.QueueCommand(new ClearSelectedChessBoardObjectCommand());
            }
            
            return commandState;
        }
    }
}