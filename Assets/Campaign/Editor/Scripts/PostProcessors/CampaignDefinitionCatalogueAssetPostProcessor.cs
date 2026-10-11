using CelesteEditor.Objects;
using QC.Campaign.Catalogue;
using QC.Campaign.Objects;

namespace QCEditor.Campaign
{
    public class CampaignDefinitionCatalogueAssetPostProcessor : CataloguePostProcessor<CampaignDefinition, CampaignDefinitionCatalogue>
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets, 
            string[] deletedAssets, 
            string[] movedAssets,
            string[] movedFromAssetPaths,
            bool didDomainReload)
        {
            HandleOnPostprocessAllAssets(importedAssets, deletedAssets, movedAssets, movedFromAssetPaths, didDomainReload);
        }
    }
}