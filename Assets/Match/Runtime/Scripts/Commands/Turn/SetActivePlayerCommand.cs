using System.Threading;
using Cysharp.Threading.Tasks;

namespace QC.Match.Commands
{
    public readonly struct SetActivePlayerCommand : ICommand<BoardCommandArgs>
    {
        private readonly int PlayerIndex;

        public SetActivePlayerCommand(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }
        
        public UniTask<CommandState> Execute(BoardCommandArgs args, CancellationToken token)
        {
            ITurnStrategy.Args turnStrategyArgs = args.ToTurnStrategyArgs(PlayerIndex);
            IWinCondition.Args winConditionArgs = args.ToWinConditionArgs(PlayerIndex);
            args.MatchState.SetActivePlayerIndex(PlayerIndex, turnStrategyArgs, winConditionArgs);

            return UniTask.FromResult(CommandState.Completed);
        }

        public override string ToString()
        {
            return $"{nameof(SetActivePlayerCommand)} - Player Index: {PlayerIndex}";
        }
    }
}