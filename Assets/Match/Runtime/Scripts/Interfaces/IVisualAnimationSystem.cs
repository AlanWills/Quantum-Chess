using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match
{
    public interface IVisualAnimationSystem
    {
        UniTask AnimateTo(ChessBoardObjectInstance objectInstance, Vector2 position);
    }
}