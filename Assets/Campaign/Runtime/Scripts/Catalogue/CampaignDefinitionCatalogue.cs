using Celeste.Objects;
using QC.Campaign.Objects;
using UnityEngine;

namespace QC.Campaign.Catalogue
{
    [CreateAssetMenu(fileName = nameof(CampaignDefinitionCatalogue), menuName = "QC/Campaign/Campaign Definition Catalogue")]
    public class CampaignDefinitionCatalogue : GuidCatalogueScriptableObject<CampaignDefinition>
    {
    }
}