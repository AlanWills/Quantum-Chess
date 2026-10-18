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
    [LuaObject]
    [CreateAssetMenu(fileName = nameof(MatchScenario), menuName = "QC/Match/Match Scenario")]
    public partial class MatchScenario : ScriptableObject, IGuid, IEditorInitializable
    {
        #region Properties and Fields

        [LuaMember("name")] private string InternalName => name;

        [LuaMember("guid")]
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

        public const string kScenarioLoadFunctionName = "load";
        public const string kScenarioSetUpFunctionName = "setUp";

        private const string kDefaultScenarioScript = @"
local StandardChessMatchFactory = require(""Factories/StandardChessMatchFactory.lua"")
local CampaignFactory = require(""Factories/CampaignFactory.lua"")

local Scenario = { }

function Scenario.setUp()
    local boardHeight = 8
    local boardWidth = 8
    local deploymentZones =
    {
        Vector2Int.new(4, 1),
        Vector2Int.new(5, 1)
    }

    -- Create Board
    StandardChessMatchFactory.createBoard(boardHeight, boardWidth)
    
    -- Add Enemy Units
    StandardChessMatchFactory.addBlackKing(Vector2Int.new(4, 8))
    
    -- Load Player's Units
    CampaignFactory.loadPlayerCurrentUnits(deploymentZones)
    
    -- Set Starting Player
    StandardChessMatchFactory.setStartingPlayer(""White"")
end

function Scenario.load()
    StandardChessMatchFactory.createHumanWhitePlayer()
    StandardChessMatchFactory.createAIBlackPlayer({
        difficulty = ""Medium""
    })
end

return Scenario";

        #endregion

        public ValueTask<LuaTable> Load(ILuaRuntime luaRuntime)
        {
            return luaRuntime.ExecuteScriptAsClassAsync(scenarioScript);
        }

#if UNITY_EDITOR
        public void Editor_Initialize()
        {
            if (scenarioScript != null)
            {
                return;
            }
            
            LuaScript luaScript = CreateInstance<LuaScript>();
            luaScript.name = $"{name}.Script";
            luaScript.Text = kDefaultScenarioScript;
            EditorOnly.CreateAssetInFolder(luaScript, EditorOnly.GetAssetFolderPath(this));

            scenarioScript = luaScript;
            EditorOnly.SetDirty(this);
            EditorOnly.SaveAsset(this);
        }
#endif
    }
}