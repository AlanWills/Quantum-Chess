using Celeste.Scene;
using QC.Campaign.Record;
using QC.GameSystems;
using UnityEngine;

namespace QC.Campaign.Loading
{
    [CreateAssetMenu(fileName = nameof(CampaignScenarioContextProvider), menuName = "QC/Loading/Campaign Scenario Context Provider")]
    public class CampaignScenarioContextProvider : ContextProvider
    {
        [SerializeField] private CampaignRecord campaignRecord;
        
        public override ILoadContext Create()
        {
            string nextScenarioGuid = campaignRecord.NextScenarioGuid();
            return new LoadScenarioContext(nextScenarioGuid);
        }
    }
}