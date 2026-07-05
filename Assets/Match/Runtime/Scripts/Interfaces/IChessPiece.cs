using System.Runtime.CompilerServices;
using Celeste.Components;

namespace QC.Match
{
    public interface IChessPiece
    {
        ChessPieceColour GetPieceColour(Instance instance);
        ChessPieceType GetPieceType(Instance instance);
        int GetValue(Instance instance);
        
        void SetVerticalDirection(Instance instance, ChessPieceVerticalDirection verticalDirection);
        void SetValue(Instance instance, int value);
    }
    
    public interface IModifiableChessPiece : IChessPiece
    {
        void SetColour(Instance instance, ChessPieceColour colour);
        void SetType(Instance instance, ChessPieceType type);
    }

    public static class ChessPieceExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ChessPieceColour GetPieceColour<T>(this InterfaceHandle<T> chessPiece) where T : class, IChessPiece
        {
            return chessPiece.iFace.GetPieceColour(chessPiece.instance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ChessPieceType GetPieceType<T>(this InterfaceHandle<T> chessPiece) where T : class, IChessPiece
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
        public static void SetVerticalDirection<T>(this InterfaceHandle<T> chessPiece, ChessPieceVerticalDirection verticalDirection) where T : class, IChessPiece
        {
            chessPiece.iFace.SetVerticalDirection(chessPiece.instance, verticalDirection);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetColour(this InterfaceHandle<IModifiableChessPiece> chessPiece, ChessPieceColour colour)
        {
            chessPiece.iFace.SetColour(chessPiece.instance, colour);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetType(this InterfaceHandle<IModifiableChessPiece> chessPiece, ChessPieceType type)
        {
            chessPiece.iFace.SetType(chessPiece.instance, type);
        }
    }
}