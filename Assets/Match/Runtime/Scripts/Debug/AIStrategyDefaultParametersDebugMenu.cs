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
            using (Section("Alpha Beta"))
            {
                var alphaBeta = aiStrategyDefaultParameters.AlphaBeta;

                using (var changeScope = new GUIChangedScope())
                {
                    alphaBeta.Depth = GUIExtensions.IntField("Depth", alphaBeta.Depth);

                    if (changeScope.Changed)
                    {
                        aiStrategyDefaultParameters.AlphaBeta = alphaBeta;
                    }
                }
            }
        }
    }
}