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
            Vector2Int currentBoardPosition,
            IBoardLookup boardLookup,
            IBoardActionMask opponentBoardActionMask,
            IList<Vector2Int> validMoves);
        void CalculateValidAttacks(
            Instance instance, 
            Vector2Int currentBoardPosition,
            IBoardLookup boardLookup, 
            IBoardActionMask opponentBoardActionMask,
            IList<Vector2Int> validAttacks);
    }
    
    public static class ActionProviderExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CalculateValidMoves(
            this InterfaceHandle<IActionProvider> provider, 
            Vector2Int currentBoardPosition,
            IBoardLookup boardLookup,
            IBoardActionMask opponentBoardActionMask,
            IList<Vector2Int> validMoves)
        {
            provider.iFace.CalculateValidMoves(provider.instance, currentBoardPosition, boardLookup, opponentBoardActionMask, validMoves);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CalculateValidAttacks(
            this InterfaceHandle<IActionProvider> provider, 
            Vector2Int currentBoardPosition,
            IBoardLookup boardLookup,
            IBoardActionMask opponentBoardActionMask,
            IList<Vector2Int> validAttacks)
        {
            provider.iFace.CalculateValidAttacks(provider.instance, currentBoardPosition, boardLookup, opponentBoardActionMask, validAttacks);
        }
    }
}