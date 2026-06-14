using Cysharp.Threading.Tasks;

namespace QC.Match.Commands
{
    public readonly struct CalculatePlayerTurnStrategyCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly int PlayerIndex;

        public CalculatePlayerTurnStrategyCommand(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }
        
        public async UniTask<CommandState> Execute(ChessBoardCommandArgs args)
        {
            ITurnStrategy.Args turnStrategyArgs = new ITurnStrategy.Args(
                args.MatchState,
                args.ChessBoard,
                args.CommandExecutor,
                args.LuaRuntime);
            await args.MatchState.CalculateTurnStrategyForPlayer(PlayerIndex, turnStrategyArgs);
            
            return CommandState.Completed;
        }
    }
}