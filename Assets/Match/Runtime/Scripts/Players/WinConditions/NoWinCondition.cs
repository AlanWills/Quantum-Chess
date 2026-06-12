using Cysharp.Threading.Tasks;

namespace QC.Match.Players
{
    public readonly struct NoWinCondition : IWinCondition
    {
        public UniTask<bool> CalculateIfWinConditionIsMet(IWinCondition.Args args)
        {
            return UniTask.FromResult(false);
        }
    }
}