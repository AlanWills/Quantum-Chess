using System;
using System.Collections.Generic;
using Celeste.Components;
using Celeste.Core.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace QC.Match.Objects
{
    [CreateAssetMenu(fileName = nameof(VisualAnimationSystemUsingHandlers), menuName = "QC/Match/Animations/Visual Animation System Using Handlers")]
    public class VisualAnimationSystemUsingHandlers : ScriptableObject, IVisualAnimationSystem
    {
        #region Properties and Fields

        [SerializeField] private float defaultAnimationSpeed = 4;

        [NonSerialized] private readonly Dictionary<int, IVisualAnimationHandler> transformHandlers = new();
        [NonSerialized] private ITimeProvider timeProvider;
        
        #endregion

        public void Initialize(ITimeProvider timeProvider)
        {
            this.timeProvider = timeProvider;
            
            transformHandlers.Clear();
        }

        public void Shutdown()
        {
            transformHandlers.Clear();
        }

        public void RegisterHandlerFor(ChessBoardObjectInstance objectInstance, IVisualAnimationHandler handler)
        {
            UnityEngine.Debug.Assert(!transformHandlers.ContainsKey(objectInstance.InstanceId), $"{nameof(IVisualAnimationHandler)} already registered for transform!");
            transformHandlers[objectInstance.InstanceId] = handler;
        }

        public void DeregisterHandlerFor(ChessBoardObjectInstance objectInstance)
        {
            transformHandlers.Remove(objectInstance.InstanceId);
        }
        
        public async UniTask AnimateTo(ChessBoardObjectInstance objectInstance, Vector2 position)
        {
            if (!transformHandlers.TryGetValue(objectInstance.InstanceId, out IVisualAnimationHandler handler))
            {
                return;
            }
            
            await handler.AnimateTo(position, timeProvider, defaultAnimationSpeed);
        }
    }
}