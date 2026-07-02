using System.Runtime.CompilerServices;
using Celeste.Components;
using UnityEngine;

namespace QC.Match
{
    public interface IGrid2DTransform
    {
        Vector2Int GetPosition(Instance instance);
        void SetPosition(Instance instance, Vector2Int position);
        void SetPosition(Instance instance, int x, int y);
    }

    public static class Grid2DTransformExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int GetPosition(this InterfaceHandle<IGrid2DTransform> grid2DTransform)
        {
            return grid2DTransform.iFace.GetPosition(grid2DTransform.instance);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPosition(this InterfaceHandle<IGrid2DTransform> grid2DTransform, Vector2Int position)
        {
            grid2DTransform.iFace.SetPosition(grid2DTransform.instance, position);
        }
    }
}