using System;
using System.Collections.Generic;
using QC.Campaign.Objects;
using UnityEngine;

namespace QC.Campaign.Record
{
    [CreateAssetMenu(fileName = nameof(CampaignRecord), menuName = "QC/Campaign/Campaign Record")]
    public class CampaignRecord : ScriptableObject
    {
        #region Properties and Fields
        
        public int CurrentCampaignGuid => currentCampaignDefinition?.Guid ?? -1;
        public IReadOnlyList<int> CurrentUnits => currentUnits;
        
        [NonSerialized] private CampaignDefinition currentCampaignDefinition;
        [NonSerialized] private readonly List<int> currentUnits = new();

        #endregion

        public void Initialize()
        {
            currentCampaignDefinition = null;
            currentUnits.Clear();
        }

        public void Initialize(CampaignDefinition campaignDefinition, IReadOnlyList<int> units)
        {
            currentCampaignDefinition = campaignDefinition;

            currentUnits.Clear();
            currentUnits.AddRange(units);
        }
    }
}