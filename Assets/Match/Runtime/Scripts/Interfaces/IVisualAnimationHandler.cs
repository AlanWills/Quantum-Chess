using Celeste.Core.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace QC.Match
{
    public interface IVisualAnimationHandler
    {
        UniTask AnimateTo(Vector2 position, ITimeProvider timeProvider, float speed);
    }
}