using Celeste.FSM;
using QC.Campaign.Objects;
using QC.Campaign.Record;
using UnityEngine;

namespace QC.Campaign.Nodes
{
    [CreateNodeMenu("QC/Campaign/Start Campaign")]
    public class StartCampaignNode : FSMNode
    {
        #region Properties and Fields

        [Input, SerializeField] private CampaignDefinition campaignDefinition;
        
        [SerializeField] private CampaignRecord campaignRecord;

        #endregion

        protected override void OnEnter()
        {
            base.OnEnter();

            CampaignDefinition campaignToStart = GetInputValue<CampaignDefinition>(nameof(campaignDefinition));
            UnityEngine.Debug.Assert(campaignToStart, $"Failed to obtain a campaign to start in {nameof(StartCampaignNode)} in FSM graph {graph.name}.");
            campaignRecord.StartCampaign(campaignToStart);
        }
    }
}