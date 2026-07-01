using Celeste.Components;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace QC.Match
{
    public interface IGrid2DTransform
    {
        Vector2Int GetPosition(Instance instance);
        void SetPosition(Instance instance, Vector2Int position);
        void SetPosition(Instance instance, int x, int y);
    }
}