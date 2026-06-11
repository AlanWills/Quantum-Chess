using UnityEngine;
using Celeste.Objects;
using Lua;
using QC.Match.Objects;

namespace QC.Match.Catalogue
{
    [LuaObject(kLuaName)]
    [CreateAssetMenu(fileName = nameof(MatchScenarioCatalogue), menuName = "QC/Match/Match Scenario Catalogue")]
    public partial class MatchScenarioCatalogue : ListScriptableObject<MatchScenario>, IAutomaticImportSettings
    {
        #region Properties and Fields

        public const string kLuaName = nameof(MatchScenarioCatalogue);
        
        public AutomaticImportBehaviour ImportBehaviour => importBehaviour;

        [LuaMember("numScenarios")] private int NumScenarios => NumItems;

        [SerializeField] private AutomaticImportBehaviour importBehaviour = AutomaticImportBehaviour.ImportAssetsInCatalogueDirectoryAndSubDirectories;

        #endregion

        public MatchScenario MustFindByGuid(string guid)
        {
            MatchScenario matchScenario = FindItem(x => string.CompareOrdinal(x.Guid, guid) == 0);
            UnityEngine.Debug.Assert(matchScenario != null, $"Failed to find {nameof(MatchScenario)} with guid '{guid}'!");
            return matchScenario;
        }

        [LuaMember("getItem")]
        private MatchScenario Lua_GetItem(int index)
        {
            return GetItem(index);
        }
    }
}