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
            
            campaignRecord.StartCampaign(campaignDefinition);
        }
    }
}