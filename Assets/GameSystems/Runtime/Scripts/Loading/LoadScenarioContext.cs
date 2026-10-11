using Celeste.Scene.Events;

namespace QC.GameSystems
{
    public class LoadScenarioContext : Context
    {
        public readonly string ScenarioGuid;

        public LoadScenarioContext(string scenarioGuid)
        {
            ScenarioGuid = scenarioGuid;
        }
    }
}
