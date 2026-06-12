using Cysharp.Threading.Tasks;

namespace QC.Match.Commands
{
    public readonly struct MakePlayerActivePlayerCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly int PlayerIndex;

        public MakePlayerActivePlayerCommand(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }
        
        public async UniTask<CommandState> Execute(ChessBoardCommandArgs args)
        {
            args.MatchState.MakePlayerActive(PlayerIndex);
            
            ITurnStrategy.Args turnStrategyArgs = new ITurnStrategy.Args(
                args.MatchState,
                args.ChessBoard,
                args.CommandExecutor,
                args.LuaRuntime);
            await args.MatchState.CalculateTurnStrategyForPlayer(args.MatchState.ActivePlayerIndex, turnStrategyArgs);
            
            return CommandState.Completed;
        }
    }
}