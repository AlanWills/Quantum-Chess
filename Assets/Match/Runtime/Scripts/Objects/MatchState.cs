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
    public partial class MatchState : ScriptableObject, IPlayerInfo, IBoardActionMaskLookup
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
        private PieceColour ActiveColour => players.Get(activePlayerIndex)?.Colour ?? PieceColour.None;
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
            Initialize(-1);
        }

        public void Initialize(MatchStateDTO dto)
        {
            Initialize(dto.activePlayerIndex);
        }

        private void Initialize(int _activePlayerIndex)
        {
            activePlayerIndex = _activePlayerIndex;
            activeScenarioGuid = string.Empty;
            players.Clear();
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
        
        public async UniTask StartScenarioAsync(string scenarioGuid)
        {
            // We may set the active player as part of the setup, so we make sure we clear this to ensure that process succeeds
            activePlayerIndex = -1;
            
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

            BuildBoardActionMasks(turnStrategyArgs.Board);

            if (player.AutoThink)
            {
                await ThinkForPlayer(playerIndex, turnStrategyArgs, false);
            }

            await UniTask.WaitUntil(player, p => p.HasPerformedAllAllowedActions);
            
            BuildBoardActionMasks(turnStrategyArgs.Board);
            
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

        private void BuildBoardActionMasks(Board board)
        {
            for (int i = 0, n = NumPlayers; i < n; ++i)
            {
                BuildBoardActionMaskForPlayer(i, board); 
            }
        }

        private void BuildBoardActionMaskForPlayer(int playerIndex, Board board)
        {
            Player player = players[playerIndex];
            PieceColour playerColour = player.Colour;
            RectInt boundingBox = board.Bounds;

            player.EnsureBoardActionMaskMatches(boundingBox);
            UpdateBoardActionMaskForColour(player.BoardActionMask, playerColour, board);
        }

        private void UpdateBoardActionMaskForColour(
            BoardActionMask actionMaskToUpdate,
            PieceColour colour, 
            Board board)
        {
            List<Vector2Int> validThreats = new List<Vector2Int>();

            actionMaskToUpdate.ResetAllTiles();
            
            foreach (BoardObjectInstance instance in board.Objects)
            {
                if (!instance.Piece.IsValid)
                {
                    continue;
                }

                if (instance.Piece.GetPieceColour() != colour)
                {
                    continue;
                }

                if (!instance.ActionProvider.IsValid || !instance.Grid2DTransform.IsValid)
                {
                    continue;
                }

                validThreats.Clear();

                Vector2Int position = instance.Grid2DTransform.GetPosition();
                instance.ActionProvider.CalculateValidThreats(position, board, validThreats);

                foreach (Vector2Int validThreat in validThreats)
                {
#if ULTRA_DEBUG_CHECKS
                    UnityEngine.Debug.Assert(board.IsPositionInBoard(validThreat), $"Calculated an invalid threat for piece: {piece.GetPieceColour()} - {piece.GetPieceType()} - {position}!");
#endif
                    actionMaskToUpdate.SetTileAttackable(validThreat);
                }
            }
        }

        public IBoardActionMask GetBoardActionMaskForPlayer(int playerIndex)
        {
            UnityEngine.Debug.Assert(IsPlayerIndexValid(playerIndex), $"Invalid player index {playerIndex} passed into {nameof(GetBoardActionMaskForPlayer)}.");
            return players[activePlayerIndex].BoardActionMask;
        }

        public IBoardActionMask GetOpponentBoardActionMaskForPlayer(int playerIndex)
        {
            // This will break if we have more than 2 players or multiple factions/teams
            // We will need to amalgamate them all into a mask relative to the inputted player (depending on their affiliation)
            int opponentIndex = (playerIndex + 1) % NumPlayers;
            return players[opponentIndex].BoardActionMask;
        }

        public async UniTask ThinkForPlayer(int playerIndex, ITurnStrategy.Args turnStrategyArgs, bool dryRun)
        {
            if (!IsPlayerIndexValid(playerIndex))
            {
                UnityEngine.Debug.LogAssertion($"Attempting to calculate next player action with an out of range index {playerIndex}.");
                return;
            }
            
#if ULTRA_DEBUG_CHECKS
            onPlayerStartedThinking.Invoke();
#else
            onPlayerStartedThinking.InvokeSilently();
#endif
            Player player = players[playerIndex];
            await player.TurnStrategy.OnTurn(turnStrategyArgs, dryRun);
            
#if ULTRA_DEBUG_CHECKS
            onPlayerFinishedThinking.Invoke();
#else
            onPlayerFinishedThinking.InvokeSilently();
#endif
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
        
        public PieceColour GetPlayerColour(int index)
        {
            return index < players.Count ? players[index].Colour : PieceColour.None;
        }
        
        public int GetPlayerIndex(PieceColour pieceColour)
        {
            for (int i = 0, n = NumPlayers; i < n; ++i)
            {
                if (players[i].Colour == pieceColour)
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
            if (!playerConfig.TryGetEnum("colour", out PieceColour playerColour))
            {
                UnityEngine.Debug.LogAssertion($"Failed to successfully parse player colour value as a valid {nameof(PieceColour)}.");
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
            if (Enum.TryParse(colourString, out PieceColour colour))
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