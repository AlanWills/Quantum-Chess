using System.Diagnostics;
using Cysharp.Threading.Tasks;
using QC.Match.AI;
using QC.Match.Commands;

namespace QC.Match.Players
{
    public class AlphaBetaTurnStrategy : ITurnStrategy
    {
        private readonly AlphaBetaParameters parameters;
        private readonly Stopwatch turnCalculationTimer = new();
        
        public AlphaBetaTurnStrategy(AlphaBetaParameters parameters)
        {
            this.parameters = parameters;
        }

        public UniTask OnTurnBegin(ITurnStrategy.Args args)
        {
            return UniTask.CompletedTask;
        }

        public async UniTask<ICommand<ChessBoardCommandArgs>> CalculateNextAction(ITurnStrategy.Args args)
        {
            turnCalculationTimer.Restart();

            await UniTask.SwitchToThreadPool();
            bool calculatedMoveSuccessfully = AlphaBeta.TryCalculate(args, parameters, out var command);
            await UniTask.SwitchToMainThread();
            
            turnCalculationTimer.Stop();

            int remainingTime = parameters.MinThinkingTime - (int)turnCalculationTimer.ElapsedMilliseconds;
            if (remainingTime > 0)
            {
                await UniTask.Delay(remainingTime);
            }

            if (calculatedMoveSuccessfully)
            {
                return command;
            }
            
            UnityEngine.Debug.LogAssertion($"Failed to calculate any valid action for colour {args.PlayerColour}.  This will result in a skipped turn...");
            return null;
        }

        public UniTask OnTurnEnd(ITurnStrategy.Args args)
        {
            return UniTask.CompletedTask;
        }
    }
}