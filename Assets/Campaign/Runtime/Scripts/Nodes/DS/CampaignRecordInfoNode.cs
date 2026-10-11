using Celeste.DS;
using QC.Campaign.Record;
using UnityEngine;
using XNode;

namespace QC.Campaign.Nodes
{
    [CreateNodeMenu("QC/Campaign/Campaign Record Info")]
    public class CampaignRecordInfoNode : DataNode
    {
        #region Properties and Fields

        [Output, SerializeField] private bool hasCurrentCampaign;

        [SerializeField] private CampaignRecord campaignRecord;

        #endregion

        public override object GetValue(NodePort port)
        {
            if (string.CompareOrdinal(port.fieldName, nameof(hasCurrentCampaign)) == 0)
            {
                return campaignRecord.HasCurrentCampaign;
            }
            
            return base.GetValue(port);
        }
    }
}
