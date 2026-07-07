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
            List<ChessBoardObjectInstance> activePlayerPieces = new List<ChessBoardObjectInstance>();
            ChessPieceColour activeColour = args.MatchState.ActiveColour;
            activePlayerPieces.AddRange(args.ChessBoard.Objects.Where(x =>
                x.HasComponent<IActionProvider>() &&
                x.HasComponent<IGrid2DTransform>() &&
                x.TryFindComponent<IChessPiece>(out var chessPiece) &&
                chessPiece.GetPieceColour() == activeColour));

            turnCalculationTimer.Restart();

            await UniTask.SwitchToThreadPool();
            bool calculatedMoveSuccessfully =
                AlphaBeta.TryCalculate(args, activePlayerPieces, parameters, out var command);
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