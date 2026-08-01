using System.Collections.Generic;

namespace QC.Match.AI.Internals
{
    public struct MoveOrderingComparer : IComparer<Move>
    {
        public int Compare(Move x, Move y)
        {
            // We want descending order (highest score first)
            return GuessMoveScore(y).CompareTo(GuessMoveScore(x));
        }

        private int GuessMoveScore(Move move)
        {
            int score = 0;

            // MVV-LVA: Most Valuable Victim - Least Valuable Attacker
            if (move.PieceToCapture.IsValid)
            {
                // Multiply the victim's value by a large number so capturing is always prioritized
                // Subtract the attacker's value so cheaper pieces capturing are checked first
                score = (100 * move.PieceToCapture.Value) - move.PieceToMove.Value;
            }

            return score;
        }
    }
}