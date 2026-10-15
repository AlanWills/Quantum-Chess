using System;
using System.Collections.Generic;
using Celeste.Objects;
using Lua;
using QC.Campaign.Objects;
using UnityEngine;

namespace QC.Campaign.Record
{
    [LuaObject(kLuaName)]
    [CreateAssetMenu(fileName = nameof(CampaignRecord), menuName = "QC/Campaign/Campaign Record")]
    public partial class CampaignRecord : ScriptableObject
    {
        #region Properties and Fields

        public const string kLuaName = nameof(CampaignRecord);
        
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
        
        #region Lua

        [LuaMember("getCurrentUnits")]
        private LuaTable Lua_GetCurrentUnits()
        {
            LuaTable luaTable = new LuaTable(currentUnits.Count, 0);
            for (int i = 0, n = currentUnits.Count; i < n; i++)
            {
                luaTable[i + 1] = currentUnits[i];
            }

            return luaTable;
        }
        
        #endregion
    }
}