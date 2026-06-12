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
            return UniTask.FromResult(args.ChessBoard.DoesChessPieceExist(OppositionColour, ChessPieceType.King));
        }
    }
}