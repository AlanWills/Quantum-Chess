using Cysharp.Threading.Tasks;

namespace QC.Match.Players
{
    public class CheckmateWinCondition : IWinCondition
    {
        private readonly ChessPieceColour OppositionColour;
        
        public CheckmateWinCondition(ChessPieceColour oppositionColour)
        {
            OppositionColour = oppositionColour;
        }
        
        public UniTask<bool> CalculateIfWinConditionIsMet(IWinCondition.Args args)
        {
            if (!args.ChessBoard.TryFindChessPiece(OppositionColour, ChessPieceType.King, out var oppositionKing))
            {
                // The King straight up does not exist, game over!
                return UniTask.FromResult(true);
            }

            if (!oppositionKing.TryFindComponent<IActionProvider>(out var oppositionKingActionProvider))
            {
                
            }
        }
    }
}