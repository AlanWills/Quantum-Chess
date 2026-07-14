namespace QC.Match.Players
{
    public class Player
    {
        public ChessPieceColour Colour { get; }
        public ITurnStrategy TurnStrategy { get; }
        public IWinCondition WinCondition { get; }
        public bool AutoEndTurn { get; }

        public Player(ChessPieceColour colour, ITurnStrategy turnStrategy, IWinCondition winCondition, bool autoEndTurn)
        {
            Colour = colour;
            TurnStrategy = turnStrategy;
            WinCondition = winCondition;
            AutoEndTurn = autoEndTurn;
        }
    }
}