using Cysharp.Threading.Tasks;

namespace QC.Match.Commands
{
    public readonly struct MakeNextPlayerActivePlayerCommand : ICommand<ChessBoardCommandArgs>
    {
        public async UniTask<CommandState> Execute(ChessBoardCommandArgs args)
        {
            args.MatchState.MakeNextPlayerActive();
            
            args.CommandExecutor.QueueCommand(new EnableInputForChessPieceColourCommand(args.MatchState.ActiveColour));
            args.CommandExecutor.QueueCommand(new DisableInputIfNotChessPieceColourCommand(args.MatchState.ActiveColour));

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