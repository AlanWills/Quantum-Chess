using System;
using UnityEngine;

namespace QC.Match.AI.Internals
{
    public readonly struct Move : IComparable<Move>
    {
        public bool IsValid => PositionX != DestinationX && PositionY != DestinationY && PieceToMove.IsValid;
           
        public readonly Piece PieceToMove;
        public readonly Piece PieceToCapture;
        public readonly int PositionX;
        public readonly int PositionY;
        public readonly int DestinationX;
        public readonly int DestinationY;

        public Move(Piece pieceToMove, Piece pieceToCapture, Vector2Int position, Vector2Int destination)
            : this(pieceToMove, pieceToCapture, position.x, position.y, destination.x, destination.y)
        {
        }

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