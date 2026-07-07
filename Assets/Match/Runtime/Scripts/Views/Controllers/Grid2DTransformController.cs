using Celeste.Components;
using Celeste.Core.Interfaces;
using Celeste.Maths;
using Cysharp.Threading.Tasks;
using QC.Match.Components;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Views
{
    public class Grid2DTransformController : InterfaceController<IGrid2DTransform, ChessBoardObjectComponent, ChessBoardObjectInstance>,
         IVisualAnimationHandler
    {
        #region Properties and Fields

        [SerializeField] private VisualAnimationSystemUsingHandlers visualAnimationSystem;
        [SerializeField] private Transform gridTransform;
        
        #endregion
        
        protected override void DoHookup(IRuntimeAddedContext context)
        {
            SyncPosition();
            
            visualAnimationSystem.RegisterHandlerFor(Instance, this);
        }

        protected override void DoShutdown()
        {
            visualAnimationSystem.DeregisterHandlerFor(Instance);
        }

        protected override void OnInterfaceDataChanged()
        {
            SyncPosition();
        }

        private void SyncPosition()
        {
            gridTransform.localPosition = Interface.GetPosition().ToVector3();
        }

        public async UniTask AnimateTo(Vector2 position, ITimeProvider timeProvider, float speed)
        {
            Vector2 startingPosition = Interface.GetPosition();
            Vector2 delta = position - startingPosition;
            float magnitude = delta.magnitude;
            float animationTime = magnitude / speed;
            float currentZ = gridTransform.localPosition.z;
            float startingTime = timeProvider.Time;
            
            AnimationCurve xAnimCurve = AnimationCurve.Linear(
                0, startingPosition.x,
                1, position.x);
            AnimationCurve yAnimCurve = AnimationCurve.Linear(
                0, startingPosition.y,
                1, position.y);

            while (true)
            {
                await UniTask.Yield(PlayerLoopTiming.Update);

                float deltaT = timeProvider.Time - startingTime;
                if (deltaT > animationTime)
                {
                    break;
                }
                
                float t = Mathf.Clamp01(deltaT / animationTime);
                Vector3 newPosition = new Vector3(xAnimCurve.Evaluate(t), yAnimCurve.Evaluate(t), currentZ);
                gridTransform.localPosition = newPosition;
            }
        }
    }
}