using Celeste.Scene;

namespace QC.GameSystems
{
    public class LoadScenarioContext : ILoadContext
    {
        public readonly string ScenarioGuid;

        public LoadScenarioContext(string scenarioGuid)
        {
            ScenarioGuid = scenarioGuid;
        }
    }
}
