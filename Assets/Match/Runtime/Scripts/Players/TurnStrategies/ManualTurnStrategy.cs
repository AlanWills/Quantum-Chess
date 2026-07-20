using Cysharp.Threading.Tasks;
using QC.Match.Commands;

namespace QC.Match.Players
{
    public readonly struct ManualTurnStrategy : ITurnStrategy
    {
        public UniTask OnTurnBegin(ITurnStrategy.Args args)
        {
            args.CommandExecutor.QueueCommand(new ToggleInputForChessPieceColourCommand(args.PlayerColour, true));
            args.CommandExecutor.QueueCommand(new ToggleInputIfNotChessPieceColourCommand(args.PlayerColour, false));
            
            return UniTask.CompletedTask;
        }

        public UniTask<ICommand<ChessBoardCommandArgs>> CalculateNextAction(ITurnStrategy.Args args)
        {
            return UniTask.FromResult<ICommand<ChessBoardCommandArgs>>(new EmptyCommand());
        }

        public UniTask OnTurnEnd(ITurnStrategy.Args args)
        {
            args.CommandExecutor.QueueCommand(new ToggleInputForChessPieceColourCommand(args.PlayerColour, false));

            return UniTask.CompletedTask;
        }
    }
}