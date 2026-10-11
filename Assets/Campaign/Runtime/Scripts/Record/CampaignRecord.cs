using System;
using System.Collections.Generic;
using Celeste.Objects;
using QC.Campaign.Objects;
using UnityEngine;

namespace QC.Campaign.Record
{
    [CreateAssetMenu(fileName = nameof(CampaignRecord), menuName = "QC/Campaign/Campaign Record")]
    public class CampaignRecord : ScriptableObject
    {
        #region Properties and Fields

        public bool HasCurrentCampaign => currentCampaignDefinition != null;
        public string CurrentCampaignGuid => HasCurrentCampaign ? currentCampaignDefinition.Guid : IGuid.Invalid;
        public IReadOnlyList<int> CurrentUnits => currentUnits;

        [SerializeField] private Celeste.Events.Event onChanged;
        
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

        public void StartCampaign(CampaignDefinition campaignDefinition)
        {
            UnityEngine.Debug.Assert(campaignDefinition != null, $"Attempting to start a campaign, but it is null!");
            Initialize(campaignDefinition, campaignDefinition.StartingUnits);
            
            onChanged.Invoke();
        }

        public string NextScenarioGuid()
        {
            // TODO, need to track which scenarios we have completed
            return HasCurrentCampaign ? currentCampaignDefinition.Scenarios[0] : IGuid.Invalid;
        }
    }
}