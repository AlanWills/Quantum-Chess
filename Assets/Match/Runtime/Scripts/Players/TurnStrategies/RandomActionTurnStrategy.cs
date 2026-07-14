using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using QC.Match.AI;
using QC.Match.Commands;
using QC.Match.Objects;

namespace QC.Match.Players
{
    public class RandomActionTurnStrategy : ITurnStrategy
    {
        public UniTask OnTurnBegin(ITurnStrategy.Args args)
        {
            return UniTask.CompletedTask;
        }

        public UniTask<ICommand<ChessBoardCommandArgs>> CalculateNextAction(ITurnStrategy.Args args)
        {
            List<ChessBoardObjectInstance> activePlayerPieces = new List<ChessBoardObjectInstance>();
            ChessPieceColour activeColour = args.MatchState.ActiveColour;
            activePlayerPieces.AddRange(args.ChessBoard.Objects.Where(x =>
                x.HasComponent<IActionProvider>() &&
                x.HasComponent<IGrid2DTransform>() &&
                x.TryFindComponent<IChessPiece>(out var chessPiece) &&
                chessPiece.GetPieceColour() == activeColour));
            
            if (RandomAction.TryCalculate(args, activePlayerPieces, out var command))
            {
                return UniTask.FromResult(command);
            }

            UnityEngine.Debug.LogAssertion($"Failed to calculate any valid move or attack for colour {args.MatchState.ActiveColour}.  Queueing up a change of colour...");
            return UniTask.FromResult<ICommand<ChessBoardCommandArgs>>(null);
        }

        public UniTask OnTurnEnd(ITurnStrategy.Args args)
        {
            return UniTask.CompletedTask;
        }
    }
}