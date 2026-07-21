namespace QC.Match.Players
{
    public class Player
    {
        public ChessPieceColour Colour { get; }
        public ITurnStrategy TurnStrategy { get; }
        public IWinCondition WinCondition { get; }
        public bool AutoThink { get; set; }
        public bool AutoEndTurn { get; set; }
        public int AllowedActionsPerTurn { get; }
        public int ActionsTakenThisTurn { get; set; }
        public bool HasPerformedAllAllowedActions => ActionsTakenThisTurn >= AllowedActionsPerTurn;
        public BoardActionMask BoardActionMask { get; set; }

        public Player(
            ChessPieceColour colour, 
            ITurnStrategy turnStrategy, 
            IWinCondition winCondition, 
            bool autoThink, 
            bool autoEndTurn,
            int allowedActionsPerTurn,
            BoardActionMask boardActionMask)
        {
            Colour = colour;
            TurnStrategy = turnStrategy;
            WinCondition = winCondition;
            AutoThink = autoThink;
            AutoEndTurn = autoEndTurn;
            AllowedActionsPerTurn = allowedActionsPerTurn;
            BoardActionMask = boardActionMask;
        }
    }
}