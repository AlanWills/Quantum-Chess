using CelesteEditor.DataStructures;
using QC.Campaign.Catalogue;
using QC.Campaign.Objects;
using UnityEditor;
using UnityEngine;

namespace QCEditor.Campaign.Catalogue
{
    [CustomEditor(typeof(CampaignDefinitionCatalogue))]
    public class CampaignDefinitionCatalogueEditor : IIndexableItemsEditor<CampaignDefinition>
    {
    }
}