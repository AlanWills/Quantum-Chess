using Cysharp.Threading.Tasks;

namespace QC.Match.Commands
{
    public readonly struct CheckPlayerWonScenarioCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly int PlayerIndex;

        public CheckPlayerWonScenarioCommand(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }
        
        public async UniTask<CommandState> Execute(ChessBoardCommandArgs args)
        {
            IWinCondition.Args winConditionArgs = new IWinCondition.Args(
                args.MatchState,
                args.ChessBoard,
                args.LuaRuntime);
            bool isScenarioWon = await args.MatchState.CalculateIfPlayerHasWon(PlayerIndex, winConditionArgs);

            UnityEngine.Debug.Assert(!isScenarioWon, $"Active Player {args.MatchState.ActiveColour} has Won!");
            return CommandState.Completed;
        }
    }
}