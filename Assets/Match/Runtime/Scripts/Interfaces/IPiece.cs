using System.Runtime.CompilerServices;
using Celeste.Components;

namespace QC.Match
{
    public interface IPiece
    {
        PieceColour GetPieceColour(Instance instance);
        PieceType GetPieceType(Instance instance);
        int GetValue(Instance instance);
        
        void SetColour(Instance instance, PieceColour colour);
        void SetType(Instance instance, PieceType type);
        void SetValue(Instance instance, int value);
        void SetVerticalDirection(Instance instance, PieceVerticalDirection verticalDirection);
    }

    public static class ChessPieceExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PieceColour GetPieceColour(this InterfaceHandle<IPiece> chessPiece)
        {
            return chessPiece.iFace.GetPieceColour(chessPiece.instance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PieceType GetPieceType(this InterfaceHandle<IPiece> chessPiece)
        {
            return chessPiece.iFace.GetPieceType(chessPiece.instance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetValue(this InterfaceHandle<IPiece> chessPiece)
        {
            return chessPiece.iFace.GetValue(chessPiece.instance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(this InterfaceHandle<IPiece> chessPiece, int value)
        {
            chessPiece.iFace.SetValue(chessPiece.instance, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetVerticalDirection(this InterfaceHandle<IPiece> chessPiece, PieceVerticalDirection verticalDirection)
        {
            chessPiece.iFace.SetVerticalDirection(chessPiece.instance, verticalDirection);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetColour(this InterfaceHandle<IPiece> chessPiece, PieceColour colour)
        {
            chessPiece.iFace.SetColour(chessPiece.instance, colour);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetType(this InterfaceHandle<IPiece> chessPiece, PieceType type)
        {
            chessPiece.iFace.SetType(chessPiece.instance, type);
        }
    }
}