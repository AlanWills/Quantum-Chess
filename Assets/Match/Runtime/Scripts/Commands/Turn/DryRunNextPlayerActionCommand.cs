using System.Threading;
using Cysharp.Threading.Tasks;

namespace QC.Match.Commands
{
    public readonly struct DryRunNextPlayerActionCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly int playerIndex;

        public DryRunNextPlayerActionCommand(int playerIndex)
        {
            this.playerIndex = playerIndex;
        }

        public async UniTask<CommandState> Execute(ChessBoardCommandArgs args, CancellationToken cancellationToken)
        {
            ITurnStrategy.Args turnStrategyArgs = args.ToTurnStrategyArgs();
            ICommand<ChessBoardCommandArgs> nextPlayerAction = await args.MatchState.CalculateNextPlayerAction(playerIndex, turnStrategyArgs);

            return CommandState.Completed;
        }
    }
}