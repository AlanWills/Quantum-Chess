using Cysharp.Threading.Tasks;
using QC.Match.Commands;

namespace QC.Match.Players
{
    public readonly struct ManualTurnStrategy : ITurnStrategy
    {
        public UniTask CalculateTurnStrategy(ITurnStrategy.Args args)
        {
            args.CommandExecutor.QueueCommand(new EnableInputForChessPieceColourCommand(args.MatchState.ActiveColour));
            args.CommandExecutor.QueueCommand(new DisableInputIfNotChessPieceColourCommand(args.MatchState.ActiveColour));
            
            return UniTask.CompletedTask;
        }
    }
}