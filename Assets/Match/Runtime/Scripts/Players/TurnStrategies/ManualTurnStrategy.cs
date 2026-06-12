using Cysharp.Threading.Tasks;
using QC.Match.Commands;

namespace QC.Match.Players
{
    public readonly struct ManualTurnStrategy : ITurnStrategy
    {
        public UniTask CalculateTurnStrategy(ITurnStrategy.Args args)
        {
            args.CommandExecutor.QueueCommand(new EndTurnCommand());
            
            return UniTask.CompletedTask;
        }
    }
}