using System.Diagnostics;
using Cysharp.Threading.Tasks;
using QC.Match.AI;

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

        public async UniTask OnTurn(ITurnStrategy.Args args, bool dryRun)
        {
            turnCalculationTimer.Restart();

            await UniTask.SwitchToThreadPool();
            bool calculatedMoveSuccessfully = AlphaBeta.TryCalculate(args, parameters, out var command);
            await UniTask.SwitchToMainThread();
            
            turnCalculationTimer.Stop();
            
            if (!calculatedMoveSuccessfully)
            {
                UnityEngine.Debug.LogAssertion($"Failed to calculate any valid action for colour {args.PlayerColour}.  This will result in a skipped turn...");
                return;
            }
            
            UnityEngine.Debug.Log($"Alpha Beta Move ({args.PlayerColour}): {command}");
            
            int remainingTime = parameters.MinThinkingTime - (int)turnCalculationTimer.ElapsedMilliseconds;
            if (remainingTime > 0)
            {
                await UniTask.Delay(remainingTime);
            }

            if (!dryRun)
            {
                args.CommandExecutor.QueueCommand(command);
            }
        }

        public UniTask OnTurnEnd(ITurnStrategy.Args args)
        {
            return UniTask.CompletedTask;
        }
    }
}