using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Celeste.DataStructures;
using Celeste.Lua;
using Cysharp.Threading.Tasks;
using Lua;
using Lua.Unity;
using QC.Match.Catalogue;
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
        public ChessPieceColour ActiveColour => ActivePlayer.Colour;
        [LuaMember("activeColour")] private string ActiveColourString => ActiveColour.ToString();
        private Player ActivePlayer => players.Get(activePlayerIndex);

        [SerializeField] private LuaRuntime luaRuntime;
        [SerializeField] private MatchScenarioCatalogue matchScenarioCatalogue;
        
        [Header("Events")] 
        [SerializeField] private Celeste.Events.Event onChanged;

        [NonSerialized] private readonly List<Player> players = new();
        [NonSerialized] private int activePlayerIndex;
        [NonSerialized] private string activeScenarioGuid;
        [NonSerialized] private LuaTable activeScenarioTable;

        #endregion

        public void Initialize()
        {
            activePlayerIndex = 0;
            activeScenarioGuid = string.Empty;
            activeScenarioTable = null;
        }

        public void Initialize(MatchStateDTO dto)
        {
            activePlayerIndex = dto.activePlayerIndex;
            activeScenarioGuid = string.Empty;
            activeScenarioTable = null;
        }

        public async UniTask<bool> LoadScenario(string scenarioGuid)
        {
            if (string.IsNullOrEmpty(scenarioGuid))
            {
                return false;
            }
            
            MatchScenario matchScenario = matchScenarioCatalogue.MustFindByGuid(scenarioGuid);
            activeScenarioGuid = scenarioGuid;
            activeScenarioTable = await matchScenario.Load(luaRuntime);
            
            var loadScenarioFunction = activeScenarioTable.GetFunction(MatchScenario.kScenarioLoadFunctionName);
            if (loadScenarioFunction != LuaValue.Nil)
            {
                await luaRuntime.ExecuteFunctionAsync(loadScenarioFunction);
            }

            bool loadedSuccessfully = activeScenarioTable != null;
            UnityEngine.Debug.Assert(loadedSuccessfully, $"Failed to load {nameof(MatchScenario)} '{matchScenario.name}'.");
            return loadedSuccessfully;
        }

        public void MakeNextPlayerActive()
        {
            activePlayerIndex = (activePlayerIndex + 1) % players.Count;
            onChanged.Invoke();
        }

        public void UpdateBasedOnActivePlayer(ChessBoardObjectInstance chessBoardObjectInstance)
        {
            if (chessBoardObjectInstance.TryFindComponent<IInteractable>(out var interactable) &&
                chessBoardObjectInstance.TryFindComponent<IChessPiece>(out var chessPiece))
            {
                bool isActiveColour = chessPiece.iFace.GetPieceColour(chessPiece.instance) == ActiveColour;
                interactable.iFace.SetInteractable(interactable.instance, isActiveColour);
            }
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
        private async ValueTask StartScenario(string scenarioGuid)
        {
            if (!await LoadScenario(scenarioGuid))
            {
                return;
            }

            var setUpScenarioFunction = activeScenarioTable.GetFunction(MatchScenario.kScenarioSetUpFunctionName);
            if (setUpScenarioFunction == LuaValue.Nil)
            {
                return;
            }

            await luaRuntime.ExecuteFunctionAsync(setUpScenarioFunction);
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