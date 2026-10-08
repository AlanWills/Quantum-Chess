using Cysharp.Threading.Tasks;
using QC.Match.Commands;

namespace QC.Match.Players
{
    public readonly struct ManualTurnStrategy : ITurnStrategy
    {
        public UniTask OnTurnBegin(ITurnStrategy.Args args)
        {
            args.CommandExecutor.QueueCommand(new ToggleInputForPieceColourCommand(args.PlayerColour, true));
            args.CommandExecutor.QueueCommand(new ToggleInputIfNotPieceColourCommand(args.PlayerColour, false));
            
            return UniTask.CompletedTask;
        }

        public UniTask OnTurn(ITurnStrategy.Args args, bool dryRun)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnTurnEnd(ITurnStrategy.Args args)
        {
            args.CommandExecutor.QueueCommand(new ToggleInputForPieceColourCommand(args.PlayerColour, false));

            return UniTask.CompletedTask;
        }
    }
}