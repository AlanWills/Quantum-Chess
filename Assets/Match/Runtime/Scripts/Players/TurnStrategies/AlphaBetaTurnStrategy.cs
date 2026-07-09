using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cysharp.Threading.Tasks;
using QC.Match.AI;
using QC.Match.Commands;
using QC.Match.Objects;

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

        public async UniTask OnTurnBegin(ITurnStrategy.Args args)
        {
            turnCalculationTimer.Restart();

            await UniTask.SwitchToThreadPool();
            bool calculatedMoveSuccessfully =
                AlphaBeta.TryCalculate(args, parameters, out var command);
            await UniTask.SwitchToMainThread();
            
            turnCalculationTimer.Stop();

            int remainingTime = parameters.MinThinkingTime - (int)turnCalculationTimer.ElapsedMilliseconds;
            if (remainingTime > 0)
            {
                await UniTask.Delay(remainingTime);
            }
            
            if (calculatedMoveSuccessfully)
            {
                args.CommandExecutor.QueueCommand(command);
            }
            else
            {
                UnityEngine.Debug.LogAssertion($"Failed to calculate any valid move or attack for colour {args.MatchState.ActiveColour}.  Queueing up a change of colour...");
                args.CommandExecutor.QueueCommand(new SetActivePlayerCommand(args.MatchState.NextPlayerIndex));
            }
        }

        public UniTask OnTurnEnd(ITurnStrategy.Args args)
        {
            return UniTask.CompletedTask;
        }
    }
}