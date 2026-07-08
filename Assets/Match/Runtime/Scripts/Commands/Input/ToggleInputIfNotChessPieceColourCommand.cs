using System.Threading;
using Cysharp.Threading.Tasks;

namespace QC.Match.Commands
{
    public readonly struct ToggleInputIfNotChessPieceColourCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly ChessPieceColour colour;
        private readonly bool inputEnabled;

        public ToggleInputIfNotChessPieceColourCommand(ChessPieceColour colour, bool inputEnabled)
        {
            this.colour = colour;
            this.inputEnabled = inputEnabled;
        }
        
        public async UniTask<CommandState> Execute(ChessBoardCommandArgs args, CancellationToken token)
        {
            var commandState = await InputCommandUtilities.SetInteractableForNotColour(args.ChessBoard, colour, inputEnabled);
            
            if (commandState == CommandState.Completed)
            {
                // Clear the selection when changing what pieces are interactable
                CommandState clearSelectedChessBoardObjectResult = await args.CommandExecutor.ExecuteCommandImmediately(new ClearSelectedChessBoardObjectCommand(), args);
                UnityEngine.Debug.Assert(clearSelectedChessBoardObjectResult != CommandState.Abandoned, $"There was an issue clearing the selected chess board object after toggling input for all but {colour}.");
            }
            
            return commandState;
        }
    }
}