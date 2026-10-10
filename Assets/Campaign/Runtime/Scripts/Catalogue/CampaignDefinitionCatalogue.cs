using Celeste.Objects;
using QC.Campaign.Objects;
using UnityEngine;

namespace QC.Campaign.Catalogue
{
    [CreateAssetMenu(fileName = nameof(CampaignDefinitionCatalogue), menuName = "QC/Campaign/Campaign Definition Catalogue")]
    public class CampaignDefinitionCatalogue : ListScriptableObject<CampaignDefinition>
    {
        public CampaignDefinition FindItemByGuid(int guid)
        {
            return FindItem(x => x.Guid == guid);
        }

        public CampaignDefinition MustFindItemByGuid(int guid)
        {
            CampaignDefinition campaignDefinition = FindItemByGuid(guid);
            UnityEngine.Debug.Assert(campaignDefinition != null, $"Failed to find {nameof(CampaignDefinition)} with guid '{guid}'.");
            return campaignDefinition;
        }
    }
}