using System.Threading.Tasks;

namespace QC.Match.Commands
{
    public struct CheckScenarioWonCommand : ICommand<ChessBoardCommandArgs>
    {
        private Task<bool> isScenarioWonTask;
        
        public CommandState Execute(ChessBoardCommandArgs args)
        {
            isScenarioWonTask ??= args.MatchState.IsScenarioWon();

            while (!isScenarioWonTask.IsCompleted)
            {
                return CommandState.Running;
            }

            return CommandState.Completed;
        }
    }
}