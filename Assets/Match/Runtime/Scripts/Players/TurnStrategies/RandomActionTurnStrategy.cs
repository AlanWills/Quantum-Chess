using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using QC.Match.AI;
using QC.Match.Objects;

namespace QC.Match.Players
{
    public class RandomActionTurnStrategy : ITurnStrategy
    {
        public UniTask OnTurnBegin(ITurnStrategy.Args args)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnTurn(ITurnStrategy.Args args, bool dryRun)
        {
            List<BoardObjectInstance> activePlayerPieces = new List<BoardObjectInstance>();
            PieceColour activeColour = args.PlayerColour;
            activePlayerPieces.AddRange(args.Board.Objects.Where(x =>
                x.HasComponent<IActionProvider>() &&
                x.HasComponent<IGrid2DTransform>() &&
                x.TryFindComponent<IChessPiece>(out var chessPiece) &&
                chessPiece.GetPieceColour() == activeColour));
            
            if (!RandomAction.TryCalculate(args, activePlayerPieces, out var command))
            {
                UnityEngine.Debug.LogAssertion($"Failed to calculate any valid move or attack for colour {args.PlayerColour}.  Queueing up a change of colour...");
                return UniTask.CompletedTask;
            }

            if (!dryRun)
            {
                args.CommandExecutor.QueueCommand(command);
            }

            return UniTask.CompletedTask;
        }

        public UniTask OnTurnEnd(ITurnStrategy.Args args)
        {
            return UniTask.CompletedTask;
        }
    }
}