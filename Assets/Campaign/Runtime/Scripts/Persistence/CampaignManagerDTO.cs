using System;
using System.Collections.Generic;
using Celeste.Persistence;
using QC.Campaign.Record;
using UnityEngine;

namespace QC.Campaign.Persistence
{
    [Serializable]
    public class CampaignManagerDTO : VersionedDTO
    {
        public int CurrentCampaignGuid => currentCampaignGuid;

        [SerializeField] private int currentCampaignGuid;
        [SerializeField] private List<int> currentUnits = new();
        
        public CampaignManagerDTO(CampaignRecord campaignRecord)
        {
            currentCampaignGuid = campaignRecord.CurrentCampaignGuid;
            currentUnits.AddRange(campaignRecord.CurrentUnits);
        }
    }
}