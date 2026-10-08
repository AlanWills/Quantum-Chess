using System.Runtime.CompilerServices;
using Celeste.Components;

namespace QC.Match
{
    public interface IChessPiece
    {
        PieceColour GetPieceColour(Instance instance);
        PieceType GetPieceType(Instance instance);
        int GetValue(Instance instance);
        
        void SetVerticalDirection(Instance instance, PieceVerticalDirection verticalDirection);
        void SetValue(Instance instance, int value);
    }
    
    public interface IModifiableChessPiece : IChessPiece
    {
        void SetColour(Instance instance, PieceColour colour);
        void SetType(Instance instance, PieceType type);
    }

    public static class ChessPieceExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PieceColour GetPieceColour<T>(this InterfaceHandle<T> chessPiece) where T : class, IChessPiece
        {
            return chessPiece.iFace.GetPieceColour(chessPiece.instance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PieceType GetPieceType<T>(this InterfaceHandle<T> chessPiece) where T : class, IChessPiece
        {
            return chessPiece.iFace.GetPieceType(chessPiece.instance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetValue<T>(this InterfaceHandle<T> chessPiece) where T : class, IChessPiece
        {
            return chessPiece.iFace.GetValue(chessPiece.instance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue<T>(this InterfaceHandle<T> chessPiece, int value) where T : class, IChessPiece
        {
            chessPiece.iFace.SetValue(chessPiece.instance, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetVerticalDirection<T>(this InterfaceHandle<T> chessPiece, PieceVerticalDirection verticalDirection) where T : class, IChessPiece
        {
            chessPiece.iFace.SetVerticalDirection(chessPiece.instance, verticalDirection);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetColour(this InterfaceHandle<IModifiableChessPiece> chessPiece, PieceColour colour)
        {
            chessPiece.iFace.SetColour(chessPiece.instance, colour);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetType(this InterfaceHandle<IModifiableChessPiece> chessPiece, PieceType type)
        {
            chessPiece.iFace.SetType(chessPiece.instance, type);
        }
    }
}