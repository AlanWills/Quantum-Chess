using System.Threading;
using Cysharp.Threading.Tasks;

namespace QC.Match.Commands
{
    public readonly struct CheckPlayerWinConditionCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly int playerIndex;

        public CheckPlayerWinConditionCommand(int playerIndex)
        {
            this.playerIndex = playerIndex;
        }

        public async UniTask<CommandState> Execute(ChessBoardCommandArgs args, CancellationToken cancellationToken)
        {
            IWinCondition.Args winConditionArgs = args.ToWinConditionArgs();
            await args.MatchState.CheckPlayerWinCondition(playerIndex, winConditionArgs);

            return CommandState.Completed;
        }
    }
}