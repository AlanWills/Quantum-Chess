using Celeste.Components;
using UnityEngine;

namespace QC.Match
{
    public interface I2DTransform
    {
        Vector2 GetPosition(Instance instance);
        void SetPosition(Instance instance, Vector2 position);
    }
}