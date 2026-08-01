using System;
using Celeste.Debug.Menus;
using Celeste.Tools;
using QC.Match.AI;
using UnityEngine;

namespace QC.Match.Debug
{
    [CreateAssetMenu(fileName = nameof(AIStrategyDefaultParametersDebugMenu), menuName = "QC/Match/Debug/AI Strategy Default Parameters Debug Menu")]
    public class AIStrategyDefaultParametersDebugMenu : DebugMenu
    {
        #region Properties and Fields

        [SerializeField] private AIStrategyDefaultParameters aiStrategyDefaultParameters;

        #endregion

        protected override void OnDrawMenu()
        {
            DrawAlphaBetaParameters(
                "Alpha Beta - Easy", 
                aiStrategyDefaultParameters.AlphaBeta_Easy,
                parameters => aiStrategyDefaultParameters.AlphaBeta_Easy = parameters);
            
            DrawAlphaBetaParameters(
                "Alpha Beta - Medium", 
                aiStrategyDefaultParameters.AlphaBeta_Medium,
                parameters => aiStrategyDefaultParameters.AlphaBeta_Medium = parameters);
            
            DrawAlphaBetaParameters(
                "Alpha Beta - Hard", 
                aiStrategyDefaultParameters.AlphaBeta_Hard,
                parameters => aiStrategyDefaultParameters.AlphaBeta_Hard = parameters);
            
            DrawAlphaBetaParameters(
                "Alpha Beta - Expert", 
                aiStrategyDefaultParameters.AlphaBeta_Expert,
                parameters => aiStrategyDefaultParameters.AlphaBeta_Expert = parameters);
        }

        private void DrawAlphaBetaParameters(
            string sectionName,
            AlphaBetaParameters parameters,
            Action<AlphaBetaParameters> onChange)
        {
            using (Section(sectionName))
            {
                using (var changeScope = new GUIChangedScope())
                {
                    parameters.Depth = GUIExtensions.IntField("Depth", parameters.Depth);
                    parameters.MinThinkingTime = GUIExtensions.IntField("Min Thinking Time", parameters.MinThinkingTime);
                    parameters.BlunderChance = GUIExtensions.FloatField("Blunder Chance", parameters.BlunderChance, 0, 1);
                    
                    // Should probably add read only paginated information on overrides here

                    if (changeScope.Changed)
                    {
                        onChange(parameters);
                    }
                }
            }
        }
    }
}