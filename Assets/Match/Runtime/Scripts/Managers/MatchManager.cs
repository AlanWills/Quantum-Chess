using QC.Match.Commands;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Managers
{
    public class MatchManager : MonoBehaviour
    {
        #region Properties and Fields

        [SerializeField] private ChessBoardCommandExecutor chessBoardCommandExecutor;
        [SerializeField] private ChessBoard chessBoard;
        [SerializeField] private ChessBoardObjectSelection chessBoardObjectSelection;
        
        #endregion
        
        #region Unity Methods

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