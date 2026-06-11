using Cysharp.Threading.Tasks;

namespace QC.Match.Commands
{
    public readonly struct CheckScenarioLostCommand : ICommand<ChessBoardCommandArgs>
    {
        public async UniTask<CommandState> Execute(ChessBoardCommandArgs args)
        {
            bool isScenarioLost = await args.MatchState.IsScenarioLost();

            UnityEngine.Debug.Assert(!isScenarioLost, $"Scenario Lost!");
            return CommandState.Completed;
        }
    }
}