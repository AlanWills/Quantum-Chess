using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Celeste.DataStructures;
using Celeste.Lua;
using Cysharp.Threading.Tasks;
using Lua;
using Lua.Unity;
using QC.Match.Catalogue;
using QC.Match.Commands;
using QC.Match.Persistence;
using QC.Match.Players;
using UnityEngine;

namespace QC.Match.Objects
{
    [LuaObject(kLuaName)]
    [CreateAssetMenu(fileName = nameof(MatchState), menuName = "QC/Match/Match State")]
    public partial class MatchState : ScriptableObject
    {
        #region Properties and Fields

        public const string kLuaName = nameof(MatchState);

        [LuaMember("hasActiveScenario")] private bool HasActiveScenario => !string.IsNullOrEmpty(activeScenarioGuid);
        [LuaMember("numPlayers")] private int NumPlayers => players.Count;

        [LuaMember("activeScenarioGuid")]
        public string ActiveScenarioGuid
        {
            get => activeScenarioGuid;
            private set
            {
                if (string.CompareOrdinal(activeScenarioGuid, value) != 0)
                {
                    activeScenarioGuid = value;
                    onChanged.Invoke();
                }
            }
        }

        private bool HasActivePlayer => 0 <= activePlayerIndex && activePlayerIndex < NumPlayers;
        [LuaMember("activePlayerIndex")] public int ActivePlayerIndex => activePlayerIndex;
        [LuaMember("nextPlayerIndex")] public int NextPlayerIndex => (activePlayerIndex + 1) % NumPlayers;
        public ChessPieceColour ActiveColour => ActivePlayer?.Colour ?? ChessPieceColour.Unknown;
        public ChessPieceColour NextColour => NextPlayer?.Colour ?? ChessPieceColour.Unknown;
        [LuaMember("activeColour")] private string ActiveColourString => ActiveColour.ToString();
        private Player ActivePlayer => players.Get(activePlayerIndex);
        private Player NextPlayer => players.Get(NextPlayerIndex);

        [SerializeField] private LuaRuntime luaRuntime;
        [SerializeField] private MatchScenarioCatalogue matchScenarioCatalogue;
        
        [Header("Events")] 
        [SerializeField] private Celeste.Events.Event onChanged;

        [NonSerialized] private readonly List<Player> players = new();
        [NonSerialized] private int activePlayerIndex = -1;
        [NonSerialized] private string activeScenarioGuid;
        [NonSerialized] private LuaTable activeScenarioTable;

        #endregion

        public void Initialize()
        {
            activePlayerIndex = -1;
            activeScenarioGuid = string.Empty;
            activeScenarioTable = null;
            players.Clear();
        }

        public void Initialize(MatchStateDTO dto)
        {
            activePlayerIndex = dto.activePlayerIndex;
            activeScenarioGuid = dto.activeScenarioGuid;
            activeScenarioTable = null;
            players.Clear();
        }

        public async UniTask TryLoadLastActiveScenario(ITurnStrategy.Args turnStrategyArgs)
        {
            if (!HasActiveScenario)
            {
                return;
            }

            if (!await LoadScenarioAsync(activeScenarioGuid))
            {
                return;
            }

            if (!HasActivePlayer)
            {
                return;
            }

            await ActivePlayer.TurnStrategy.OnTurnBegin(turnStrategyArgs);
        }

        private async UniTask<bool> LoadScenarioAsync(string scenarioGuid)
        {
            if (string.IsNullOrEmpty(scenarioGuid))
            {
                UnityEngine.Debug.LogAssertion("Attempting to load a scenario with an empty guid.  Ignoring...");
                return false;
            }
            
            players.Clear();

            MatchScenario matchScenario = matchScenarioCatalogue.MustFindByGuid(scenarioGuid);
            activeScenarioGuid = scenarioGuid;
            activeScenarioTable = await matchScenario.Load(luaRuntime);
            
            var loadScenarioFunction = activeScenarioTable.GetFunction(MatchScenario.kScenarioLoadFunctionName);
            if (loadScenarioFunction != null)
            {
                await luaRuntime.ExecuteFunctionAsync(loadScenarioFunction);
            }

            bool loadedSuccessfully = activeScenarioTable != null;
            UnityEngine.Debug.Assert(loadedSuccessfully, $"Failed to load {nameof(MatchScenario)} '{matchScenario.name}'.");
            return loadedSuccessfully;
        }
        
        private async UniTask StartScenarioAsync(string scenarioGuid)
        {
            if (!await LoadScenarioAsync(scenarioGuid))
            {
                return;
            }

            var setUpScenarioFunction = activeScenarioTable.GetFunction(MatchScenario.kScenarioSetUpFunctionName);
            if (setUpScenarioFunction == null)
            {
                return;
            }

            await luaRuntime.ExecuteFunctionAsync(setUpScenarioFunction);
        }

        public async UniTask SetActivePlayerIndex(
            int playerIndex, 
            ITurnStrategy.Args turnStrategyArgs, 
            IWinCondition.Args winConditionArgs)
        {
            if (HasActivePlayer)
            {
                await ActivePlayer.TurnStrategy.OnTurnEnd(turnStrategyArgs);
                
                bool isScenarioWon = await ActivePlayer.WinCondition.CalculateIfWinConditionIsMet(winConditionArgs);
                UnityEngine.Debug.Assert(!isScenarioWon, $"Active Player {ActiveColour} has Won!");
            }
            
            activePlayerIndex = playerIndex;
            onChanged.Invoke();

            if (HasActivePlayer)
            {
                await ActivePlayer.TurnStrategy.OnTurnBegin(turnStrategyArgs);
            }
        }

        public MatchStateDTO Serialize()
        {
            return new MatchStateDTO(this);
        }
        
        #region Lua

        [LuaMember("startScenario")]
        private void StartScenario(string scenarioGuid)
        {
            StartScenarioAsync(scenarioGuid).Forget();
        }

        [LuaMember("addPlayer")]
        private void AddPlayer(LuaTable playerConfig)
        {
            if (!playerConfig.TryGetEnum("colour", out ChessPieceColour playerColour))
            {
                UnityEngine.Debug.LogAssertion($"Failed to successfully parse player colour value as a valid {nameof(ChessPieceColour)}.");
                return;
            }

            if (!playerConfig.TryGetEnum("turnStrategy", out TurnStrategyFactory.Type turnStrategyType))
            {
                UnityEngine.Debug.LogAssertion("Failed to successfully parse turn strategy value.");
                return;
            }

            if (!playerConfig.TryGetEnum("winCondition", out WinConditionFactory.Type winConditionType))
            {
                UnityEngine.Debug.LogAssertion("Failed to successfully parse player win condition value.");
                return;
            }

            ITurnStrategy turnStrategy = TurnStrategyFactory.Create(turnStrategyType, playerConfig);
            IWinCondition winCondition = WinConditionFactory.Create(winConditionType, playerConfig);
            Player player = new Player(playerColour, turnStrategy, winCondition);
            players.Add(player);
        }

        [LuaMember("findPlayerIndexOfColour")]
        private int FindPlayerIndexOfColour(string colourString)
        {
            if (Enum.TryParse(colourString, out ChessPieceColour colour))
            {
                return players.FindIndex(x => x.Colour == colour);
            }

            return -1;
        }
        
        #endregion
    }
}