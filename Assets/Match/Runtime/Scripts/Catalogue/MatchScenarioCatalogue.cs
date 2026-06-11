using UnityEngine;
using Celeste.Objects;
using QC.Match.Objects;

namespace QC.Match.Catalogue
{
    [CreateAssetMenu(fileName = nameof(MatchScenarioCatalogue), menuName = "QC/Match/Match Scenario Catalogue")]
    public class MatchScenarioCatalogue : ListScriptableObject<MatchScenario>, IAutomaticImportSettings
    {
        #region Properties and Fields

        public AutomaticImportBehaviour ImportBehaviour => importBehaviour;

        [SerializeField] private AutomaticImportBehaviour importBehaviour = AutomaticImportBehaviour.ImportAssetsInCatalogueDirectoryAndSubDirectories;

        #endregion

        public MatchScenario MustFindByGuid(string guid)
        {
            MatchScenario matchScenario = FindItem(x => string.CompareOrdinal(x.Guid, guid) == 0);
            UnityEngine.Debug.Assert(matchScenario != null, $"Failed to find {nameof(MatchScenario)} with guid '{guid}'!");
            return matchScenario;
        }
    }
}