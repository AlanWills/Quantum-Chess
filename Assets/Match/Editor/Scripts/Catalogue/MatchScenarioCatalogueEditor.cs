using UnityEngine;
using UnityEditor;
using CelesteEditor.DataStructures;
using QC.Match.Objects;
using QC.Match.Catalogue;

namespace QCEditor.Match.Catalogue
{
    [CustomEditor(typeof(MatchScenarioCatalogue))]
    public class MatchScenarioCatalogueEditor : IIndexableItemsEditor<MatchScenario>
    {
    }
}