using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Celeste.Components;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match
{
    public interface IActionProvider
    {
        void CalculateValidMoves(
            Instance instance, 
            int currentPositionX,
            int currentPositionY,
            IBoardLookup boardLookup,
            IBoardActionMask opponentBoardActionMask,
            IList<Vector2Int> validMoves);
        void CalculateValidAttacks(
            Instance instance, 
            int currentPositionX,
            int currentPositionY,
            IBoardLookup boardLookup, 
            IBoardActionMask opponentBoardActionMask,
            IList<Vector2Int> validAttacks);
        void CalculateValidThreats(
            Instance instance, 
            int currentPositionX,
            int currentPositionY,
            IBoardLookup boardLookup,
            IList<Vector2Int> validThreats);
    }
    
    public static class ActionProviderExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CalculateValidMoves(
            this InterfaceHandle<IActionProvider> provider, 
            int currentPositionX,
            int currentPositionY,
            IBoardLookup boardLookup,
            IBoardActionMask opponentBoardActionMask,
            IList<Vector2Int> validMoves)
        {
            provider.iFace.CalculateValidMoves(provider.instance, currentPositionX, currentPositionY, boardLookup, opponentBoardActionMask, validMoves);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CalculateValidAttacks(
            this InterfaceHandle<IActionProvider> provider, 
            int currentPositionX,
            int currentPositionY,
            IBoardLookup boardLookup,
            IBoardActionMask opponentBoardActionMask,
            IList<Vector2Int> validAttacks)
        {
            provider.iFace.CalculateValidAttacks(provider.instance, currentPositionX, currentPositionY, boardLookup, opponentBoardActionMask, validAttacks);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CalculateValidThreats(
            this InterfaceHandle<IActionProvider> provider, 
            int currentPositionX,
            int currentPositionY,
            IBoardLookup boardLookup,
            IList<Vector2Int> validThreats)
        {
            provider.iFace.CalculateValidThreats(provider.instance, currentPositionX, currentPositionY, boardLookup, validThreats);
        }
    }
}