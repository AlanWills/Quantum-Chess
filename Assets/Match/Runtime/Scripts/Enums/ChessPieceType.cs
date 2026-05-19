using System;

namespace QC.Match
{
    [Flags]
    public enum ChessPieceType
    {
        None = 0,
        Pawn = 1 << 0,
        Knight = 1 << 1,
        Bishop = 1 << 2,
        Rook = 1 << 3,
        Queen = 1 << 4,
        King = 1 << 5,
    }

    public static class ChessPieceTypeExtensions
    {
        public const ChessPieceType Everything =
            ChessPieceType.Pawn |
            ChessPieceType.Knight |
            ChessPieceType.Bishop |
            ChessPieceType.Rook |
            ChessPieceType.Queen | 
            ChessPieceType.King;
    }
}