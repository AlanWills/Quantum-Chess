using System.Threading.Tasks;
using Celeste.Lua;
using Celeste.Objects;
using Celeste.Tools;
using Celeste.Tools.Attributes.GUI;
using Lua;
using Lua.Unity;
using UnityEngine;

namespace QC.Match.Objects
{
    [CreateAssetMenu(fileName = nameof(MatchScenario), menuName = "QC/Match/Match Scenario")]
    public class MatchScenario : ScriptableObject, IGuid
    {
        #region Properties and Fields

        public string Guid
        {
            get => guid;
            set
            {
                if (string.CompareOrdinal(guid, value) != 0)
                {
                    guid = value;
                    EditorOnly.SetDirty(this);
                }
            }
        }

        [SerializeField, StringAsSystemGuid] private string guid;
        [SerializeField] private LuaScript scenarioScript;

        public const string kScenarioSetUpFunctionName = "setUp";
        public const string kCheckScenarioWonFunctionName = "checkScenarioWon";
        public const string kCheckScenarioLostFunctionName = "checkScenarioLost";

        #endregion

        public ValueTask<LuaTable> Load(ILuaRuntime luaRuntime)
        {
            return luaRuntime.ExecuteScriptAsClassAsync(scenarioScript);
        }
    }
}