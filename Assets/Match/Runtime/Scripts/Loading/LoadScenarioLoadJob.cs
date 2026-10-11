using System;
using System.Collections;
using Celeste.Loading;
using UnityEngine;

namespace QC.Match.Loading
{
    [CreateAssetMenu(fileName = nameof(LoadScenarioLoadJob), menuName = "QC/Loading/Load Scenario")]
    public class LoadScenarioLoadJob : LoadJob
    {
        public override IEnumerator Execute(Action<float> setProgress, Action<string> setOutput)
        {
            throw new NotImplementedException();
        }
    }
}