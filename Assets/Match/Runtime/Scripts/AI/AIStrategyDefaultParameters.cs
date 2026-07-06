using System;
using Celeste.Tools.Attributes.GUI;
using QC.Match.Players;
using UnityEngine;

namespace QC.Match.AI
{
    [CreateAssetMenu(fileName = nameof(AIStrategyDefaultParameters), menuName = "QC/Match/AI/AI Strategy Default Parameters")]
    public class AIStrategyDefaultParameters : ScriptableObject
    {
        #region Properties and Fields

        public AlphaBetaParameters AlphaBeta_Easy
        {
            get => runtimeAlphaBetaEasy;
            set => runtimeAlphaBetaEasy = value;
        }
        
        public AlphaBetaParameters AlphaBeta_Medium
        {
            get => runtimeAlphaBetaMedium;
            set => runtimeAlphaBetaMedium = value;
        }
        
        public AlphaBetaParameters AlphaBeta_Hard
        {
            get => runtimeAlphaBetaHard;
            set => runtimeAlphaBetaHard = value;
        }
        
        public AlphaBetaParameters AlphaBeta_Expert
        {
            get => runtimeAlphaBetaExpert;
            set => runtimeAlphaBetaExpert = value;
        }

        [SerializeField, InlineDataInInspector] private AlphaBetaParameters defaultAlphaBetaEasy;
        [SerializeField, InlineDataInInspector] private AlphaBetaParameters defaultAlphaBetaMedium;
        [SerializeField, InlineDataInInspector] private AlphaBetaParameters defaultAlphaBetaHard;
        [SerializeField, InlineDataInInspector] private AlphaBetaParameters defaultAlphaBetaExpert;

        [NonSerialized] private AlphaBetaParameters runtimeAlphaBetaEasy;
        [NonSerialized] private AlphaBetaParameters runtimeAlphaBetaMedium;
        [NonSerialized] private AlphaBetaParameters runtimeAlphaBetaHard;
        [NonSerialized] private AlphaBetaParameters runtimeAlphaBetaExpert;
        
        #endregion

        public void Initialize()
        {
            runtimeAlphaBetaEasy = defaultAlphaBetaEasy;
            runtimeAlphaBetaMedium = defaultAlphaBetaMedium;
            runtimeAlphaBetaHard = defaultAlphaBetaHard;
            runtimeAlphaBetaExpert = defaultAlphaBetaExpert;
        }

        public AlphaBetaParameters AlphaBetaParametersFromAIDifficulty(TurnStrategyFactory.AIDifficulty difficulty)
        {
            switch (difficulty)
            {
                case TurnStrategyFactory.AIDifficulty.Easy:
                    return AlphaBeta_Easy;
                
                case TurnStrategyFactory.AIDifficulty.Medium:
                    return AlphaBeta_Medium;
                
                case TurnStrategyFactory.AIDifficulty.Hard:
                    return AlphaBeta_Hard;
                
                case TurnStrategyFactory.AIDifficulty.Expert:
                    return AlphaBeta_Expert;

                default:
                    UnityEngine.Debug.LogAssertion($"Unhandled difficulty type '{difficulty}' when attempting to fetch Alpha Beta Parameters.");
                    return AlphaBeta_Medium;
            }
        }
    }
}