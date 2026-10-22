using System;
using System.Collections;
using Celeste.Loading;
using Celeste.Scene;
using Cysharp.Threading.Tasks;
using QC.GameSystems;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Loading
{
    [CreateAssetMenu(fileName = nameof(StartScenarioLoadTask), menuName = "QC/Loading/Start Scenario Async")]
    public class StartScenarioLoadTask : LoadTask
    {
        [SerializeField] private MatchState matchState;
        
        public override UniTask Execute(ILoadContext context, Action<float> setProgress, Action<string> setOutput)
        {
            LoadScenarioContext loadScenarioContext = context as LoadScenarioContext;
            UnityEngine.Debug.Assert(loadScenarioContext != null, $"Attempting to call {nameof(StartScenarioLoadJob)} {name} without a valid {nameof(LoadScenarioContext)}.");
            return matchState.StartScenarioAsync(loadScenarioContext.ScenarioGuid);
        }
    }
}