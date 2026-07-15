using System.Threading;
using Cysharp.Threading.Tasks;

namespace QC.Match.Commands
{
    public readonly struct SetActivePlayerCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly int PlayerIndex;

        public SetActivePlayerCommand(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }
        
        public async UniTask<CommandState> Execute(ChessBoardCommandArgs args, CancellationToken token)
        {
            ITurnStrategy.Args turnStrategyArgs = args.ToTurnStrategyArgs();
            IWinCondition.Args winConditionArgs = args.ToWinConditionArgs();
            await args.MatchState.SetActivePlayerIndex(PlayerIndex, turnStrategyArgs, winConditionArgs);

            return CommandState.Completed;
        }
    }
}