using System.Threading;
using Cysharp.Threading.Tasks;

namespace QC.Match.Commands
{
    public readonly struct ExecuteNextPlayerActionCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly int playerIndex;
        private readonly bool dryRun;

        public ExecuteNextPlayerActionCommand(int playerIndex, bool dryRun)
        {
            this.playerIndex = playerIndex;
            this.dryRun = dryRun;
        }

        public async UniTask<CommandState> Execute(ChessBoardCommandArgs args, CancellationToken cancellationToken)
        {
            ITurnStrategy.Args turnStrategyArgs = args.ToTurnStrategyArgs(playerIndex);
            ICommand<ChessBoardCommandArgs> nextPlayerAction = await args.MatchState.CalculateNextPlayerAction(playerIndex, turnStrategyArgs);

            if (nextPlayerAction == null)
            { 
                UnityEngine.Debug.LogAssertion($"Failed to calculate the next action for player {playerIndex}.  Did you mean to use {nameof(EmptyCommand)}?");
                return CommandState.Abandoned;
            }
            
            if (dryRun)
            {
                return CommandState.Completed;
            }

            turnStrategyArgs.CommandExecutor.QueueCommand(nextPlayerAction);
            return CommandState.Completed;
        }
    }
}