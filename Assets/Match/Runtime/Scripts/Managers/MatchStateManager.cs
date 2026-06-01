using Celeste.Persistence;
using QC.Match.Commands;
using QC.Match.Objects;
using QC.Match.Persistence;
using UnityEngine;
using UnityEngine.Events;

namespace QC.Match.Managers
{
    public class MatchStateManager : PersistentSceneManager<MatchStateManager, MatchStateDTO>
    {
        #region Properties and Fields

        public const string FILE_NAME = "match_state.dat";

        protected override string FileName => FILE_NAME;

        [SerializeField] private MatchState matchState;
        [SerializeField] private ChessBoardCommandExecutor chessBoardCommandExecutor;
        [SerializeField] private ChessBoard chessBoard;
        [SerializeField] private ChessBoardObjectSelection chessBoardObjectSelection;
        [SerializeField] private UnityEvent onMatchSetup = new();
        [SerializeField] private UnityEvent onMatchShutdown = new();
        
        #endregion
        
        #region Unity Methods
        
        private void OnEnable()
        {
            onMatchSetup.Invoke();
        }

        private void OnDisable()
        {
            onMatchShutdown.Invoke();
        }

        private void Update()
        {
            ChessBoardCommandArgs args = CreateCommandArgs();
            chessBoardCommandExecutor.UpdateCommands(args);
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

        private ChessBoardCommandArgs CreateCommandArgs()
        {
            return new ChessBoardCommandArgs(
                matchState,
                chessBoardCommandExecutor,
                chessBoard,
                chessBoardObjectSelection);
        }
    }
}