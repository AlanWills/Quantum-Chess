using Cysharp.Threading.Tasks;
using QC.Match.Commands;

namespace QC.Match.Players
{
    public readonly struct NoTurnStrategy : ITurnStrategy
    {
        public UniTask OnTurnBegin(ITurnStrategy.Args args)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnTurn(ITurnStrategy.Args args, bool dryRun)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnTurnEnd(ITurnStrategy.Args args)
        {
            return UniTask.CompletedTask;
        }
    }
}