using System;
using System.Collections.Generic;
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
    public partial class MatchState : ScriptableObject, IPlayerInfo
    {
        #region Properties and Fields

        public const string kLuaName = nameof(MatchState);

        [LuaMember("hasActiveScenario")] public bool HasActiveScenario => !string.IsNullOrEmpty(activeScenarioGuid);
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
        [LuaMember("nextPlayerIndex")] private int NextPlayerIndex => (activePlayerIndex + 1) % NumPlayers;
        private ChessPieceColour ActiveColour => players.Get(activePlayerIndex)?.Colour ?? ChessPieceColour.Unknown;
        [LuaMember("activeColour")] private string ActiveColourString => ActiveColour.ToString();

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

        public async UniTask LoadLastActiveScenario(
            ITurnStrategy.Args turnStrategyArgs,
            IWinCondition.Args winConditionArgs)
        {
            if (!HasActiveScenario)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to load a scenario, but we don't actually have one active.  Did you forget to call {nameof(HasActiveScenario)}?");
                return;
            }

            await LoadScenarioAsync(activeScenarioGuid);
            
            BeginTurnForPlayer(ActivePlayerIndex, turnStrategyArgs, winConditionArgs);
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
            if (!IsPlayerIndexValid(playerIndex))
            {
                UnityEngine.Debug.LogAssertion($"Attempting to check player win condition with an out of range index {playerIndex}.");
                return false;
            }

            return await players[playerIndex].WinCondition.CalculateIfWinConditionIsMet(winConditionArgs);
        }

        public void SetActivePlayerIndex(
            int playerIndex, 
            ITurnStrategy.Args turnStrategyArgs, 
            IWinCondition.Args winConditionArgs)
        {
            if (activePlayerIndex == playerIndex)
            {
                UnityEngine.Debug.LogAssertion($"You're trying to set the active player index to {playerIndex}, but it has already been set to that.  Ignoring...");
                return;
            }
            
            activePlayerIndex = playerIndex;
            
            BeginTurnForPlayer(playerIndex, turnStrategyArgs, winConditionArgs);
        }

        private void BeginTurnForPlayer(
            int playerIndex,
            ITurnStrategy.Args turnStrategyArgs,
            IWinCondition.Args winConditionArgs)
        {
            if (!IsPlayerIndexValid(playerIndex))
            {
                UnityEngine.Debug.LogAssertion($"Really serious issue here!  You're attempting to load an active player, assuming one has been set up, but it hasn't...");
                return;
            }
            
            players[playerIndex].ActionsTakenThisTurn = 0;
            onChanged.Invoke();
            
            PlayerTurn(playerIndex, turnStrategyArgs, winConditionArgs).Forget();
        }

        private async UniTask PlayerTurn(
            int playerIndex,
            ITurnStrategy.Args turnStrategyArgs, 
            IWinCondition.Args winConditionArgs)
        {
            if (!IsPlayerIndexValid(playerIndex))
            {
                UnityEngine.Debug.LogAssertion($"Failed to execute {nameof(PlayerTurn)} for player index {playerIndex} as it was an invalid index.");
                return;
            }
            
            Player player = players[playerIndex];
            
            await player.TurnStrategy.OnTurnBegin(turnStrategyArgs);

            BuildBoardActionMasks();

            if (player.AutoThink)
            {
                await ThinkForPlayer(playerIndex, turnStrategyArgs, false);
            }

            await UniTask.WaitUntil(player, p => p.HasPerformedAllAllowedActions);
            await player.TurnStrategy.OnTurnEnd(turnStrategyArgs);
                
            if (await player.WinCondition.CalculateIfWinConditionIsMet(winConditionArgs))
            {
                // Probably need to trigger an event or something here when a player has won
                UnityEngine.Debug.LogAssertion($"Player {ActiveColour} has won!");
                return;
            }

            if (player.AutoEndTurn)
            {
                turnStrategyArgs.CommandExecutor.QueueCommand(new SetActivePlayerCommand(NextPlayerIndex));
            }
        }

        private void BuildBoardActionMasks(ChessBoard chessBoard)
        {
            for (int i = 0, n = NumPlayers; i < n; ++i)
            {
                BuildBoardActionMaskForPlayer(i, chessBoard); 
            }
        }

        private void BuildBoardActionMaskForPlayer(int playerIndex, ChessBoard chessBoard)
        {
            ChessPieceColour playerColour = players[playerIndex].Colour;
            chessBoard.UpdateBoardActionMaskForColour(players[playerIndex].BoardActionMask, playerColour);
        }

        public async UniTask ThinkForPlayer(int playerIndex, ITurnStrategy.Args turnStrategyArgs, bool dryRun)
        {
            if (!IsPlayerIndexValid(playerIndex))
            {
                UnityEngine.Debug.LogAssertion($"Attempting to calculate next player action with an out of range index {playerIndex}.");
                return;
            }
            
            onPlayerStartedThinking.Invoke();
            Player player = players[playerIndex];
            await player.TurnStrategy.OnTurn(turnStrategyArgs, dryRun);
            onPlayerFinishedThinking.Invoke();
        }

        public void RecordActionTakenByPlayer(int playerIndex)
        {
            if (playerIndex < 0 || playerIndex >= NumPlayers)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to record action for player {playerIndex}, but it is an invalid index.");
                return;
            }

            ++players[playerIndex].ActionsTakenThisTurn;
        }
        
        public ChessPieceColour GetPlayerColour(int index)
        {
            return index < players.Count ? players[index].Colour : ChessPieceColour.Unknown;
        }
        
        public int GetPlayerIndex(ChessPieceColour chessPieceColour)
        {
            for (int i = 0, n = NumPlayers; i < n; ++i)
            {
                if (players[i].Colour == chessPieceColour)
                {
                    return i;
                }
            }
            
            return -1;
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

            if (!playerConfig.TryGet("autoThink", out bool autoThink))
            {
                UnityEngine.Debug.Log("Failed to find value for 'autoThink' so defaulting to 'true'.");
                autoThink = true;
            }

            if (!playerConfig.TryGet("autoEndTurn", out bool autoEndTurn))
            {
                UnityEngine.Debug.Log("Failed to find value for 'autoEndTurn' so defaulting to 'true'.");
                autoEndTurn = true;
            }

            if (!playerConfig.TryGet("actionsPerTurn", out int actionsPerTurn))
            {
                UnityEngine.Debug.Log("Failed to find value for 'actionsPerTurn' so defaulting to '1'.");
                actionsPerTurn = 1;
            }

            ITurnStrategy turnStrategy = TurnStrategyFactory.Create(turnStrategyType, aiStrategyDefaultParameters, playerConfig);
            IWinCondition winCondition = WinConditionFactory.Create(winConditionType, playerConfig);
            Player player = new Player(playerColour, turnStrategy, winCondition, autoThink, autoEndTurn, actionsPerTurn);
            players.Add(player);
        }

        [LuaMember("isPlayerIndexValid")]
        private bool IsPlayerIndexValid(int index)
        {
            return index >= 0 && index < NumPlayers;
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
        private string Lua_GetPlayerColour(int index)
        {
            return index < players.Count ? players[index].Colour.ToString() : string.Empty;
        }

        [LuaMember("getPlayerAutoThink")]
        private bool GetPlayerAutoThink(int index)
        {
            return index < players.Count && players[index].AutoThink;
        }

        [LuaMember("setPlayerAutoThink")]
        private void GetPlayerAutoThink(int index, bool newValue)
        {
            if (index < players.Count)
            {
                players[index].AutoThink = newValue;
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