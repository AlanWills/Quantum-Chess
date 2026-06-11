using Cysharp.Threading.Tasks;

namespace QC.Match.Commands
{
    public readonly struct CheckScenarioWonCommand : ICommand<ChessBoardCommandArgs>
    {
        public async UniTask<CommandState> Execute(ChessBoardCommandArgs args)
        {
            bool isScenarioWon = await args.MatchState.IsScenarioWon();

            UnityEngine.Debug.Assert(!isScenarioWon, $"Scenario Won!");
            return CommandState.Completed;
        }
    }
}