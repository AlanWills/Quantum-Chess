namespace QC.Match.Commands
{
    public readonly struct SetActiveChessPieceColourCommand : ICommand<ChessBoardCommandArgs>
    {
        public readonly ChessPieceColour ActiveColour;

        public SetActiveChessPieceColourCommand(ChessPieceColour activeColour)
        {
            ActiveColour = activeColour;
        }
        
        public CommandState Execute(ChessBoardCommandArgs args)
        {
            args.MatchState.ActiveColour = ActiveColour;
            
            return CommandState.Completed;
        }
    }
}