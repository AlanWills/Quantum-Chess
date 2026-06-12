using Cysharp.Threading.Tasks;

namespace QC.Match.Commands
{
    public readonly struct EndTurnCommand : ICommand<ChessBoardCommandArgs>
    {
        public UniTask<CommandState> Execute(ChessBoardCommandArgs args)
        {
            args.CommandExecutor.QueueCommand(new CheckPlayerWonScenarioCommand(args.MatchState.ActivePlayerIndex));
            args.CommandExecutor.QueueCommand(new MakePlayerActivePlayerCommand(args.MatchState.NextPlayerIndex));
            
            return UniTask.FromResult(CommandState.Completed);
        }
    }
}