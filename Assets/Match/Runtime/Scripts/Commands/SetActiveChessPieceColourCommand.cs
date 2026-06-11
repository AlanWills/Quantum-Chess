using Cysharp.Threading.Tasks;

namespace QC.Match.Commands
{
    public readonly struct SetActiveChessPieceColourCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly ChessPieceColour ActiveColour;

        public SetActiveChessPieceColourCommand(ChessPieceColour activeColour)
        {
            ActiveColour = activeColour;
        }
        
        public UniTask<CommandState> Execute(ChessBoardCommandArgs args)
        {
            args.MatchState.ActiveColour = ActiveColour;
            args.CommandExecutor.QueueCommand(new EnableInputForChessPieceColourCommand(ActiveColour));
            args.CommandExecutor.QueueCommand(new DisableInputIfNotChessPieceColourCommand(ActiveColour));
            
            return UniTask.FromResult(CommandState.Completed);
        }
    }
}