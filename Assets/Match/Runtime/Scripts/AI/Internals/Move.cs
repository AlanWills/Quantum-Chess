using UnityEngine;

namespace QC.Match.AI.Internals
{
    public readonly struct Move
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
    }
}