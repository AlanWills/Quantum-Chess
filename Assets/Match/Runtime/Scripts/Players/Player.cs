namespace QC.Match.Players
{
    public class Player
    {
        public ChessPieceColour Colour { get; }
        public ITurnStrategy TurnStrategy { get; }
        public IWinCondition WinCondition { get; }

        public Player(ChessPieceColour colour, ITurnStrategy turnStrategy, IWinCondition winCondition)
        {
            Colour = colour;
            TurnStrategy = turnStrategy;
            WinCondition = winCondition;
        }
    }
}