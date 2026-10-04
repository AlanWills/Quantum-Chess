using System.Threading;
using Cysharp.Threading.Tasks;

namespace QC.Match.Commands
{
    public readonly struct CheckPlayerWinConditionCommand : ICommand<BoardCommandArgs>
    {
        private readonly int playerIndex;

        public CheckPlayerWinConditionCommand(int playerIndex)
        {
            this.playerIndex = playerIndex;
        }

        public async UniTask<CommandState> Execute(BoardCommandArgs args, CancellationToken cancellationToken)
        {
            IWinCondition.Args winConditionArgs = args.ToWinConditionArgs(playerIndex);
            await args.MatchState.CheckPlayerWinCondition(playerIndex, winConditionArgs);

            return CommandState.Completed;
        }

        public override string ToString()
        {
            return $"{nameof(CheckPlayerWinConditionCommand)} - Player Index: {playerIndex}";
        }
    }
}