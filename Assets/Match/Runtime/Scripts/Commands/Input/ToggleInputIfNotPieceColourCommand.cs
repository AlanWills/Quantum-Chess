using System.Threading;
using Cysharp.Threading.Tasks;

namespace QC.Match.Commands
{
    public readonly struct ToggleInputIfNotPieceColourCommand : ICommand<BoardCommandArgs>
    {
        private readonly PieceColour colour;
        private readonly bool inputEnabled;

        public ToggleInputIfNotPieceColourCommand(PieceColour colour, bool inputEnabled)
        {
            this.colour = colour;
            this.inputEnabled = inputEnabled;
        }
        
        public async UniTask<CommandState> Execute(BoardCommandArgs args, CancellationToken token)
        {
            var commandState = await InputCommandUtilities.SetInteractableForNotColour(args.Board, colour, inputEnabled);
            
            if (commandState == CommandState.Completed)
            {
                // Clear the selection when changing what pieces are interactable
                CommandState clearSelectedChessBoardObjectResult = await args.CommandExecutor.ExecuteCommandImmediately(new ClearSelectedBoardObjectCommand(), args);
                UnityEngine.Debug.Assert(clearSelectedChessBoardObjectResult != CommandState.Abandoned, $"There was an issue clearing the selected chess board object after toggling input for all but {colour}.");
            }
            
            return commandState;
        }

        public override string ToString()
        {
            return $"{nameof(ToggleInputIfNotPieceColourCommand)} - Colour: {colour} - Input Enabled: {inputEnabled}";
        }
    }
}