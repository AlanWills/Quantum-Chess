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
    [CreateAssetMenu(fileName = nameof(StartScenarioLoadJob), menuName = "QC/Loading/Start Scenario")]
    public class StartScenarioLoadJob : LoadJob
    {
        [SerializeField] private MatchState matchState;
        
        public override IEnumerator Execute(ILoadContext context, Action<float> setProgress, Action<string> setOutput)
        {
            LoadScenarioContext loadScenarioContext = context as LoadScenarioContext;
            UnityEngine.Debug.Assert(loadScenarioContext != null, $"Attempting to call {nameof(StartScenarioLoadJob)} {name} without a valid {nameof(LoadScenarioContext)}.");
            yield return matchState.StartScenarioAsync(loadScenarioContext.ScenarioGuid).ToCoroutine();
        }
    }
}