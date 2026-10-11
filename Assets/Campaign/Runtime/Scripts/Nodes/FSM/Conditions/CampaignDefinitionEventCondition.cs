using System.ComponentModel;
using Celeste.FSM.Nodes.Events.Conditions;
using QC.Campaign.Events;
using QC.Campaign.Objects;

namespace QC.Campaign.Nodes.Events
{
    [DisplayName("Campaign Definition")]
    public class CampaignDefinitionEventCondition : ParameterizedEventCondition<CampaignDefinition, CampaignDefinitionEvent>
    {
    }
}