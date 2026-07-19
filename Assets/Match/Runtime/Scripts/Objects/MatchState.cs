using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Celeste.DataStructures;
using Celeste.Lua;
using Cysharp.Threading.Tasks;
using Lua;
using Lua.Unity;
using QC.Match.AI;
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
        [SerializeField] private AIStrategyDefaultParameters aiStrategyDefaultParameters;
        
        [Header("Events")]
        [SerializeField] private Celeste.Events.Event onBeforeScenarioSetUp;
        [SerializeField] private Celeste.Events.Event onPlayerStartedThinking;
        [SerializeField] private Celeste.Events.Event onPlayerFinishedThinking;
        [SerializeField] private Celeste.Events.Event onChanged;

        [NonSerialized] private readonly List<Player> players = new();
        [NonSerialized] private int activePlayerIndex = -1;
        [NonSerialized] private string activeScenarioGuid;
        [NonSerialized] private LuaTable activeScenarioTable;

        #endregion

        public void Initialize()
        {
            Initialize(-1, string.Empty);
        }

        public void Initialize(MatchStateDTO dto)
        {
            Initialize(dto.activePlayerIndex, dto.activeScenarioGuid);
        }

        private void Initialize(
            int activePlayerIndex,
            string activeScenarioGuid)
        {
            this.activePlayerIndex = activePlayerIndex;
            this.activeScenarioGuid = activeScenarioGuid;
            players.Clear();
        }

        public async UniTask TryLoadLastActiveScenario(
            ITurnStrategy.Args turnStrategyArgs, 
            IWinCondition.Args winConditionArgs)
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

            PerformTurnForActivePlayer(turnStrategyArgs, winConditionArgs).Forget();
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
            
            onBeforeScenarioSetUp.Invoke();

            var setUpScenarioFunction = activeScenarioTable.GetFunction(MatchScenario.kScenarioSetUpFunctionName);
            if (setUpScenarioFunction == null)
            {
                return;
            }

            await luaRuntime.ExecuteFunctionAsync(setUpScenarioFunction);
        }

        public async UniTask<bool> CheckPlayerWinCondition(int playerIndex, IWinCondition.Args winConditionArgs)
        {
            if (playerIndex < 0 || playerIndex >= NumPlayers)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to check player win condition with an out of range index {playerIndex}.");
                return false;
            }

            return await players[playerIndex].WinCondition.CalculateIfWinConditionIsMet(winConditionArgs);
        }

        public async UniTask<ICommand<ChessBoardCommandArgs>> CalculateNextPlayerAction(int playerIndex, ITurnStrategy.Args turnStrategyArgs)
        {
            if (playerIndex < 0 || playerIndex >= NumPlayers)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to calculate next player action with an out of range index {playerIndex}.");
                return new EmptyCommand();
            }
            
            onPlayerStartedThinking.Invoke();
            var nextAction = await players[playerIndex].TurnStrategy.CalculateNextAction(turnStrategyArgs);
            onPlayerFinishedThinking.Invoke();

            return nextAction;
        }

        public void SetActivePlayerIndex(
            int playerIndex, 
            ITurnStrategy.Args turnStrategyArgs, 
            IWinCondition.Args winConditionArgs)
        {
            activePlayerIndex = playerIndex;
            players[activePlayerIndex].ActionsTakenThisTurn = 0;
            
            onChanged.Invoke();

            if (HasActivePlayer)
            {
                PerformTurnForActivePlayer(turnStrategyArgs, winConditionArgs).Forget();
            }
        }

        private async UniTask PerformTurnForActivePlayer(
            ITurnStrategy.Args turnStrategyArgs, 
            IWinCondition.Args winConditionArgs)
        {
            // Ideally we'd want to pump the CommandDispatcher and make sure all commands are completed
            // However, it's possible this is being called from a command, in which case it will wait forever
            await ActivePlayer.TurnStrategy.OnTurnBegin(turnStrategyArgs);

            var nextAction = await ActivePlayer.TurnStrategy.CalculateNextAction(turnStrategyArgs);
            UnityEngine.Debug.Assert(nextAction != null, $"Attempting to return a null command from a turn strategy for player {ActivePlayerIndex}.  Did you mean to use {nameof(EmptyCommand)}?"); 
            
            if (nextAction != null && ActivePlayer.AutoQueueAction)
            {
                turnStrategyArgs.CommandExecutor.QueueCommand(nextAction);
            }

            await ActivePlayer.TurnStrategy.OnTurnEnd(turnStrategyArgs);
                
            if (await ActivePlayer.WinCondition.CalculateIfWinConditionIsMet(winConditionArgs))
            {
                // Probably need to trigger an event or something here when a player has won
                UnityEngine.Debug.LogAssertion($"Active Player {ActiveColour} has won!");
                return;
            }

            if (ActivePlayer.AutoEndTurn)
            {
                turnStrategyArgs.CommandExecutor.QueueCommand(new SetActivePlayerCommand(NextPlayerIndex));
            }
        }

        public void RecordActionForActivePlayer()
        {
            if (!HasActivePlayer)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to record action for active player, but there is no active player.");
                return;
            }

            ++ActivePlayer.ActionsTakenThisTurn;
        }

        public int GetNumActionsTakenByActivePlayer()
        {
            return HasActivePlayer ? ActivePlayer.ActionsTakenThisTurn : 0;
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

            if (!playerConfig.TryGet("autoQueueAction", out bool autoQueueAction))
            {
                UnityEngine.Debug.Log("Failed to find value for 'autoQueueAction' so defaulting to 'true'.");
                autoQueueAction = true;
            }

            if (!playerConfig.TryGet("autoEndTurn", out bool autoEndTurn))
            {
                UnityEngine.Debug.Log("Failed to find value for 'autoEndTurn' so defaulting to 'true'.");
                autoEndTurn = true;
            }

            ITurnStrategy turnStrategy = TurnStrategyFactory.Create(turnStrategyType, aiStrategyDefaultParameters, playerConfig);
            IWinCondition winCondition = WinConditionFactory.Create(winConditionType, playerConfig);
            Player player = new Player(playerColour, turnStrategy, winCondition, autoQueueAction, autoEndTurn);
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

        [LuaMember("getPlayerColour")]
        private string GetPlayerColour(int index)
        {
            return index < players.Count ? players[index].Colour.ToString() : string.Empty;
        }

        [LuaMember("getPlayerAutoQueueAction")]
        private bool GetPlayerAutoQueueAction(int index)
        {
            return index < players.Count && players[index].AutoQueueAction;
        }

        [LuaMember("setPlayerAutoQueueAction")]
        private void GetPlayerAutoQueueAction(int index, bool newValue)
        {
            if (index < players.Count)
            {
                players[index].AutoQueueAction = newValue;
            }
        }

        [LuaMember("getPlayerAutoEndTurn")]
        private bool GetPlayerAutoEndTurn(int index)
        {
            return index < players.Count && players[index].AutoEndTurn;
        }

        [LuaMember("setPlayerAutoEndTurn")]
        private void GetPlayerAutoEndTurn(int index, bool newValue)
        {
            if (index < players.Count)
            {
                players[index].AutoEndTurn = newValue;
            }
        }
        
        #endregion
    }
}