using System;
using Celeste.Tools.Attributes.GUI;
using UnityEngine;

namespace QC.Match.AI
{
    [CreateAssetMenu(fileName = nameof(AIStrategyDefaultParameters), menuName = "QC/Match/AI/AI Strategy Default Parameters")]
    public class AIStrategyDefaultParameters : ScriptableObject
    {
        #region Properties and Fields

        public AlphaBetaParameters AlphaBeta
        {
            get => runtimeAlphaBeta;
            set => runtimeAlphaBeta = value;
        }
        
        [SerializeField, InlineDataInInspector] private AlphaBetaParameters defaultAlphaBeta;

        [NonSerialized] private AlphaBetaParameters runtimeAlphaBeta;
        
        #endregion

        public void Initialize()
        {
            runtimeAlphaBeta = defaultAlphaBeta;
        }
    }
}