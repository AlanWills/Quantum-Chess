using System;
using Celeste.Core;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Managers
{
    public class VisualAnimationSystemManager : MonoBehaviour
    {
        #region Properties and Fields

        [SerializeField] private VisualAnimationSystemUsingHandlers visualAnimationSystem;

        #endregion
        
        #region Unity Methods

        private void OnEnable()
        {
            visualAnimationSystem.Initialize(new UnityTimeProvider());
        }

        private void OnDisable()
        {
            visualAnimationSystem.Shutdown();
        }

        #endregion
    }
}