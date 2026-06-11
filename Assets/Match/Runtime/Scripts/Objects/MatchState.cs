using System;
using System.Threading.Tasks;
using Celeste.Lua;
using Celeste.Tools;
using Cysharp.Threading.Tasks;
using Lua;
using Lua.Unity;
using QC.Match.Catalogue;
using QC.Match.Events;
using QC.Match.Persistence;
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

        public ChessPieceColour ActiveColour
        {
            get => activeColour;
            set
            {
                if (activeColour != value)
                {
                    ChessPieceColour oldColour = activeColour;
                    activeColour = value;
                    activeColourChangedEvent.Invoke(new ActiveColourChangedArgs(oldColour, activeColour));
                    onChanged.Invoke();
                }
            }
        }

        [LuaMember("activeColour")] 
        private string ActiveColourString => ActiveColour.ToString();

        public ChessPieceColour NextActiveColour
        {
            get
            {
                switch (activeColour)
                {
                    case ChessPieceColour.White:
                        return ChessPieceColour.Black;
                    
                    case ChessPieceColour.Black:
                        return ChessPieceColour.White;
                    
                    case ChessPieceColour.Unknown:
                    default:
                        return ChessPieceColour.White;
                }
            }
        }
        
        [LuaMember("nextActiveColour")] 
        private string NextActiveColourString => NextActiveColour.ToString();

        [SerializeField] private LuaRuntime luaRuntime;
        [SerializeField] private MatchScenarioCatalogue matchScenarioCatalogue;
        
        [Header("Events")] 
        [SerializeField] private ActiveColourChangedEvent activeColourChangedEvent;
        [SerializeField] private Celeste.Events.Event onChanged;

        [NonSerialized] private ChessPieceColour activeColour;
        [NonSerialized] private string activeScenarioGuid;
        [NonSerialized] private LuaTable activeScenarioTable;

        #endregion

        public void Initialize()
        {
            activeColour = ChessPieceColour.Unknown;
            activeScenarioGuid = string.Empty;
            activeScenarioTable = null;
        }

        public void Initialize(MatchStateDTO dto)
        {
            activeColour = dto.activeColour;
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

            bool loadedSuccessfully = activeScenarioTable != null;
            UnityEngine.Debug.Assert(loadedSuccessfully, $"Failed to load {nameof(MatchScenario)} '{matchScenario.name}'.");
            return loadedSuccessfully;
        }

        public async UniTask<bool> IsScenarioWon()
        {
            var checkScenarioWonFunction = activeScenarioTable.GetFunction(MatchScenario.kCheckScenarioWonFunctionName);
            if (checkScenarioWonFunction == LuaValue.Nil)
            {
                return false;
            }

            var returnValues = await luaRuntime.ExecuteFunctionAsync(checkScenarioWonFunction);
            return returnValues.Length == 1 && returnValues[0].ToBoolean();
        }

        public async UniTask<bool> IsScenarioLost()
        {
            var checkScenarioLostFunction = activeScenarioTable.GetFunction(MatchScenario.kCheckScenarioLostFunctionName);
            if (checkScenarioLostFunction == LuaValue.Nil)
            {
                return false;
            }
            
            var returnValues = await luaRuntime.ExecuteFunctionAsync(checkScenarioLostFunction);
            return returnValues.Length == 1 && returnValues[0].ToBoolean();
        }

        public void UpdateBasedOnActiveColour(ChessBoardObjectInstance chessBoardObjectInstance)
        {
            if (chessBoardObjectInstance.TryFindComponent<IInteractable>(out var interactable) &&
                chessBoardObjectInstance.TryFindComponent<IChessPiece>(out var chessPiece))
            {
                bool isActiveColour = chessPiece.iFace.GetPieceColour(chessPiece.instance) == ActiveColour;
                interactable.iFace.SetInteractable(interactable.instance, isActiveColour);
            }
        }

        public MatchStateDTO Serialize()
        {
            return new MatchStateDTO(this);
        }
        
        #region Lua Bindings

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
        
        #endregion
    }
}