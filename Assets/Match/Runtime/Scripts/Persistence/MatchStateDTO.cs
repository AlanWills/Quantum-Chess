using System;
using Celeste.Persistence;
using QC.Match.Objects;

namespace QC.Match.Persistence
{
    [Serializable]
    public class MatchStateDTO : VersionedDTO
    {
        public ChessPieceColour activeColour;
        public string activeScenarioGuid;
        
        public MatchStateDTO(MatchState matchState)
        {
            activeColour = matchState.ActiveColour;
            activeScenarioGuid = matchState.ActiveScenarioGuid;
        }
    }
}