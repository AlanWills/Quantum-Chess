using Celeste.Lua;
using QC.Campaign.Record;
using UnityEngine;

namespace QC.Campaign
{
    public class CampaignLuaBindings : MonoBehaviour, ILuaBindings
    {
        [SerializeField] private CampaignRecord campaignRecord;
        
        public void Bind(LuaRuntime luaRuntime)
        {
            luaRuntime.SetEnvironmentVariable(CampaignRecord.kLuaName, campaignRecord);
        }

        public void Unbind(LuaRuntime luaRuntime)
        {
            luaRuntime.ClearEnvironmentVariable(CampaignRecord.kLuaName);
        }
    }
}