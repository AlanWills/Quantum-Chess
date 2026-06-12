using System;
using Celeste.Persistence;
using QC.Match.Objects;

namespace QC.Match.Persistence
{
    [Serializable]
    public class MatchStateDTO : VersionedDTO
    {
        public int activePlayerIndex;
        public string activeScenarioGuid;
        
        public MatchStateDTO(MatchState matchState)
        {
            activePlayerIndex = matchState.ActivePlayerIndex;
            activeScenarioGuid = matchState.ActiveScenarioGuid;
        }
    }
}