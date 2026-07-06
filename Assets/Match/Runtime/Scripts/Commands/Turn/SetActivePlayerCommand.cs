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
        
        public async UniTask<CommandState> Execute(ChessBoardCommandArgs args)
        {
            ITurnStrategy.Args turnStrategyArgs = new ITurnStrategy.Args(
                args.MatchState,
                args.ChessBoard,
                args.CommandExecutor,
                args.LuaRuntime,
                args.RNG);
            IWinCondition.Args winConditionArgs = new IWinCondition.Args(
                args.MatchState,
                args.ChessBoard,
                args.LuaRuntime);
            await args.MatchState.SetActivePlayerIndex(PlayerIndex, turnStrategyArgs, winConditionArgs);

            return CommandState.Completed;
        }
    }
}