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
        public readonly int DestinationImportance;

        public Move(Piece pieceToMove, Piece pieceToCapture, int positionX, int positionY, int destinationX, int destinationY, int destinationImportance)
        {
            PieceToMove = pieceToMove;
            PieceToCapture = pieceToCapture;
            PositionX = positionX;
            PositionY = positionY;
            DestinationX = destinationX;
            DestinationY = destinationY;
            DestinationImportance = destinationImportance;
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
            if (move.PieceToCapture.IsValid)
            {
                return 10000 + (100 * move.PieceToCapture.Value) - move.PieceToMove.Value;
            }

            return move.DestinationImportance;
        }
    }
}