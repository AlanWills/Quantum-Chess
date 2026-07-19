namespace QC.Match.Players
{
    public class Player
    {
        public ChessPieceColour Colour { get; }
        public ITurnStrategy TurnStrategy { get; }
        public IWinCondition WinCondition { get; }
        public bool AutoQueueAction { get; set; }
        public bool AutoEndTurn { get; set; }
        public int ActionsTakenThisTurn { get; set; }

        public Player(ChessPieceColour colour, ITurnStrategy turnStrategy, IWinCondition winCondition, bool autoQueueAction, bool autoEndTurn)
        {
            Colour = colour;
            TurnStrategy = turnStrategy;
            WinCondition = winCondition;
            AutoQueueAction = autoQueueAction;
            AutoEndTurn = autoEndTurn;
        }
    }
}