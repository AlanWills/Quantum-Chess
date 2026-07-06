using System;
using System.Collections;
using Celeste.Assets;
using Celeste.Core;
using Celeste.Core.Interfaces;
using Celeste.Coroutines;
using Celeste.Lua;
using Celeste.Persistence;
using Cysharp.Threading.Tasks;
using QC.Match.Commands;
using QC.Match.Events;
using QC.Match.Objects;
using QC.Match.Persistence;
using UnityEngine;
using UnityEngine.Events;

namespace QC.Match.Managers
{
    public class MatchStateManager : PersistentSceneManager<MatchStateManager, MatchStateDTO>, IHasAssets
    {
        #region Properties and Fields

        public const string FILE_NAME = "match_state.dat";

        protected override string FileName => FILE_NAME;

        [SerializeField] private MatchState matchState;
        [SerializeField] private ChessBoardCommandExecutor chessBoardCommandExecutor;
        [SerializeField] private ChessBoard chessBoard;
        [SerializeField] private ChessBoardObjectSelection chessBoardObjectSelection;
        [SerializeField] private VisualAnimationSystemUsingHandlers visualAnimationSystem;
        [SerializeField] private LuaRuntime luaRuntime;
        [SerializeField] private UnityEvent onMatchSetup = new();
        [SerializeField] private UnityEvent onMatchShutdown = new();

        [NonSerialized] private IRNG rng = new UnityRNG();

        #endregion
        
        #region Unity Methods

        private void OnDisable()
        {
            onMatchShutdown.Invoke();
        }
        
        #endregion
        
        #region Save/Load
        
        protected override MatchStateDTO Serialize()
        {
            return matchState.Serialize();
        }

        protected override void Deserialize(MatchStateDTO dto)
        {
            matchState.Initialize(dto);
        }

        protected override void SetDefaultValues()
        {
            matchState.Initialize();
        }
        
        #endregion

        public bool ShouldLoadAssets()
        {
            return true;
        }

        public IEnumerator LoadAssets()
        {
            ITurnStrategy.Args turnStrategyArgs = new ITurnStrategy.Args(
                matchState,
                chessBoard,
                chessBoardCommandExecutor,
                luaRuntime,
                rng);

            ChessBoardCommandArgs args = CreateCommandArgs();
            chessBoardCommandExecutor.BeginProcessing(args);
            
            yield return matchState.TryLoadLastActiveScenario(turnStrategyArgs);
            
            onMatchSetup.Invoke();
        }

        private ChessBoardCommandArgs CreateCommandArgs()
        {
            return new ChessBoardCommandArgs(
                matchState,
                chessBoardCommandExecutor,
                chessBoard,
                chessBoardObjectSelection,
                luaRuntime,
                visualAnimationSystem,
                rng);
        }
    }
}