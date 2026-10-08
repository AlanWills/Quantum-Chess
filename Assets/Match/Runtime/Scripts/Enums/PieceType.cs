using System;

namespace QC.Match
{
    [Flags]
    public enum PieceType
    {
        None = 0,
        Pawn = 1 << 0,
        Knight = 1 << 1,
        Bishop = 1 << 2,
        Rook = 1 << 3,
        Queen = 1 << 4,
        King = 1 << 5,
    }

    public static class PieceTypeExtensions
    {
        public const PieceType Everything =
            PieceType.Pawn |
            PieceType.Knight |
            PieceType.Bishop |
            PieceType.Rook |
            PieceType.Queen | 
            PieceType.King;
    }
}