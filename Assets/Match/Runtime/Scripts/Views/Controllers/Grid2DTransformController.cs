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
            gridTransform.localPosition = Interface.iFace.GetPosition(Interface.instance).ToVector3();
        }

        public async UniTask AnimateTo(Vector2 position, ITimeProvider timeProvider, float speed)
        {
            Vector2 startingPosition = Interface.iFace.GetPosition(Interface.instance);
            Vector2 delta = position - startingPosition;
            float magnitude = delta.magnitude;
            float animationTime = magnitude / speed;
            float currentTime = 0;
            float currentZ = gridTransform.localPosition.z;
            
            AnimationCurve xAnimCurve = AnimationCurve.EaseInOut(
                0, startingPosition.x,
                animationTime, position.x);
            AnimationCurve yAnimCurve = AnimationCurve.EaseInOut(
                0, startingPosition.y,
                animationTime, position.y);

            while (currentTime < animationTime)
            {
                currentTime += timeProvider.DeltaTime;
                Vector3 newPosition = new Vector3(xAnimCurve.Evaluate(currentTime), yAnimCurve.Evaluate(currentTime), currentZ);
                gridTransform.localPosition = newPosition;
                
                await UniTask.Yield();
            }
        }
    }
}