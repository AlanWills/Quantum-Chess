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

        public UniTask<ICommand<ChessBoardCommandArgs>> CalculateNextAction(ITurnStrategy.Args args)
        {
            return UniTask.FromResult<ICommand<ChessBoardCommandArgs>>(null);
        }

        public UniTask OnTurnEnd(ITurnStrategy.Args args)
        {
            return UniTask.CompletedTask;
        }
    }
}