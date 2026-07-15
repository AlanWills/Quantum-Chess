using Cysharp.Threading.Tasks;
using QC.Match.Commands;

namespace QC.Match.Players
{
    public readonly struct ManualTurnStrategy : ITurnStrategy
    {
        public UniTask OnTurnBegin(ITurnStrategy.Args args)
        {
            args.CommandExecutor.QueueCommand(new ToggleInputForChessPieceColourCommand(args.MatchState.ActiveColour, true));
            args.CommandExecutor.QueueCommand(new ToggleInputIfNotChessPieceColourCommand(args.MatchState.ActiveColour, false));
            
            return UniTask.CompletedTask;
        }

        public UniTask<ICommand<ChessBoardCommandArgs>> CalculateNextAction(ITurnStrategy.Args args)
        {
            return UniTask.FromResult<ICommand<ChessBoardCommandArgs>>(new WaitForActionsTakenByActivePlayerCommand(1));
        }

        public UniTask OnTurnEnd(ITurnStrategy.Args args)
        {
            args.CommandExecutor.QueueCommand(new ToggleInputForChessPieceColourCommand(args.MatchState.ActiveColour, false));

            return UniTask.CompletedTask;
        }
    }
}