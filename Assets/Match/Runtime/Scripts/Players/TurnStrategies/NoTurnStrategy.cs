using Cysharp.Threading.Tasks;

namespace QC.Match.Players
{
    public readonly struct NoTurnStrategy : ITurnStrategy
    {
        public UniTask CalculateTurnStrategy(ITurnStrategy.Args args)
        {
            return UniTask.CompletedTask;
        }
    }
}