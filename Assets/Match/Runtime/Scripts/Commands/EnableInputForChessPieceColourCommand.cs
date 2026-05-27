namespace QC.Match.Commands
{
    public readonly struct EnableInputForChessPieceColourCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly ChessPieceColour colour;

        public EnableInputForChessPieceColourCommand(ChessPieceColour colour)
        {
            this.colour = colour;
        }
        
        public CommandState Execute(ChessBoardCommandArgs args)
        {
            // Clear the selection when changing what pieces are interactable
            args.Selection.CurrentSelectedObject = null;
            
            foreach (var instance in args.ChessBoard.Objects)
            {
                if (!instance.TryFindComponent<IChessPiece>(out var piece) ||
                    !instance.TryFindComponent<IInteractable>(out var interactable))
                {
                    continue;
                }
                
                bool isOfMatchingColour = piece.iFace.GetPieceColour(piece.instance) == colour;
                interactable.iFace.SetInteractable(interactable.instance, isOfMatchingColour);
            }

            return CommandState.Completed;
        }
    }
}