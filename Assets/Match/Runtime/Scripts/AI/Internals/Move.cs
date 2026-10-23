using System;
using UnityEngine;

namespace QC.Match.AI.Internals
{
    public readonly struct Move : IComparable<Move>
    {
        public bool IsValid => Position != Destination && PieceToMove.IsValid;
           
        public readonly Piece PieceToMove;
        public readonly Piece PieceToCapture;
        public readonly Vector2Int Position;
        public readonly Vector2Int Destination;

        public Move(Piece pieceToMove, Piece pieceToCapture, Vector2Int position, Vector2Int destination)
        {
            PieceToMove = pieceToMove;
            PieceToCapture = pieceToCapture;
            Position = position;
            Destination = destination;
        }

        public int CompareTo(Move other)
        {
            return GuessMoveScore(other).CompareTo(GuessMoveScore(this));
        }
        
        private static int GuessMoveScore(Move move)
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