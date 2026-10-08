using UnityEngine;

namespace QC.Match.Players
{
    public class Player
    {
        public PieceColour Colour { get; }
        public ITurnStrategy TurnStrategy { get; }
        public IWinCondition WinCondition { get; }
        public bool AutoThink { get; set; }
        public bool AutoEndTurn { get; set; }
        public int AllowedActionsPerTurn { get; }
        public int ActionsTakenThisTurn { get; set; }
        public bool HasPerformedAllAllowedActions => ActionsTakenThisTurn >= AllowedActionsPerTurn;
        public BoardActionMask BoardActionMask { get; private set; }

        public Player(
            PieceColour colour, 
            ITurnStrategy turnStrategy, 
            IWinCondition winCondition, 
            bool autoThink, 
            bool autoEndTurn,
            int allowedActionsPerTurn)
        {
            Colour = colour;
            TurnStrategy = turnStrategy;
            WinCondition = winCondition;
            AutoThink = autoThink;
            AutoEndTurn = autoEndTurn;
            AllowedActionsPerTurn = allowedActionsPerTurn;
        }

        public void EnsureBoardActionMaskMatches(RectInt boundingBox)
        {
            if (BoardActionMask == null || !BoardActionMask.MatchesBoundingBox(boundingBox))
            {
                BoardActionMask = new BoardActionMask(boundingBox);
            }
        }
    }
}