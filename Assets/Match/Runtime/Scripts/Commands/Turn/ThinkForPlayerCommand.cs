using System.Threading;
using Cysharp.Threading.Tasks;

namespace QC.Match.Commands
{
    public readonly struct ThinkForPlayerCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly int playerIndex;
        private readonly bool dryRun;

        public ThinkForPlayerCommand(int playerIndex, bool dryRun)
        {
            this.playerIndex = playerIndex;
            this.dryRun = dryRun;
        }

        public async UniTask<CommandState> Execute(ChessBoardCommandArgs args, CancellationToken cancellationToken)
        {
            ITurnStrategy.Args turnStrategyArgs = args.ToTurnStrategyArgs(playerIndex);
            await args.MatchState.ThinkForPlayer(playerIndex, turnStrategyArgs, dryRun);

            return CommandState.Completed;
        }
    }
}