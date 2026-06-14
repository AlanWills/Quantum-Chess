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

        [LuaMember("activePlayerIndex")] public int ActivePlayerIndex => activePlayerIndex;
        [LuaMember("nextPlayerIndex")] public int NextPlayerIndex => (activePlayerIndex + 1) % players.Count;
        public ChessPieceColour ActiveColour => ActivePlayer?.Colour ?? ChessPieceColour.Unknown;
        [LuaMember("activeColour")] private string ActiveColourString => ActiveColour.ToString();
        private Player ActivePlayer => players.Get(activePlayerIndex);

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

        public async UniTask TryLoadLastActiveScenario(ICommandExecutor<ChessBoardCommandArgs> commandExecutor)
        {
            if (!HasActiveScenario)
            {
                return;
            }

            if (!await LoadScenarioAsync(activeScenarioGuid))
            {
                return;
            }
            
            commandExecutor.QueueCommand(new CalculatePlayerTurnStrategyCommand(ActivePlayerIndex));
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

        public void MakePlayerActive(int playerIndex)
        {
            if (playerIndex < 0 || players.Count <= playerIndex)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to make player active ({playerIndex}), but it is not a valid player index.");
                return;
            }
            
            activePlayerIndex = playerIndex;
            onChanged.Invoke();
        }

        public UniTask CalculateTurnStrategyForPlayer(int playerIndex, ITurnStrategy.Args args)
        {
            Player player = players.Get(playerIndex);
            return player != null ? player.TurnStrategy.CalculateTurnStrategy(args) : UniTask.CompletedTask;
        }

        public UniTask<bool> CalculateIfPlayerHasWon(int playerIndex, IWinCondition.Args args)
        {
            Player player = players.Get(playerIndex);
            return player != null ? player.WinCondition.CalculateIfWinConditionIsMet(args) : UniTask.FromResult(false);
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
        
        #endregion
    }
}