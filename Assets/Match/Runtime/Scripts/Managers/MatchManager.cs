using QC.Match.Commands;
using QC.Match.Objects;
using UnityEngine;
using UnityEngine.Events;

namespace QC.Match.Managers
{
    public class MatchManager : MonoBehaviour
    {
        #region Properties and Fields

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

        private ChessBoardCommandArgs CreateCommandArgs()
        {
            return new ChessBoardCommandArgs(
                chessBoardCommandExecutor,
                chessBoard,
                chessBoardObjectSelection);
        }
    }
}