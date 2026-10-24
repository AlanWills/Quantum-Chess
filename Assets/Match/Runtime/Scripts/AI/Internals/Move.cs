using System;
using UnityEngine;

namespace QC.Match.AI.Internals
{
    public readonly struct Move : IComparable<Move>
    {
        public readonly Piece PieceToMove;
        public readonly Piece PieceToCapture;
        public readonly int PositionX;
        public readonly int PositionY;
        public readonly int DestinationX;
        public readonly int DestinationY;

        public Move(Piece pieceToMove, Piece pieceToCapture, int positionX, int positionY, int destinationX, int destinationY)
        {
            PieceToMove = pieceToMove;
            PieceToCapture = pieceToCapture;
            PositionX = positionX;
            PositionY = positionY;
            DestinationX = destinationX;
            DestinationY = destinationY;
        }

        public int CompareTo(Move other)
        {
            return GuessMoveScore(other).CompareTo(GuessMoveScore(this));
        }

        public int CompareTo(in Move other)
        {
            return GuessMoveScore(in other).CompareTo(GuessMoveScore(in this));
        }
        
        private static int GuessMoveScore(in Move move)
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