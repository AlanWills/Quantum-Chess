using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match
{
    public interface IVisualAnimationSystem
    {
        UniTask AnimateTo(BoardObjectInstance objectInstance, Vector2 position);
    }
}