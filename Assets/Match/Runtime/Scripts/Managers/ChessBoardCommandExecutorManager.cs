using QC.Match.Commands;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Managers
{
    public class ChessBoardCommandExecutorManager : MonoBehaviour
    {
        #region Properties and Fields

        [SerializeField] private ChessBoard chessBoard;
        [SerializeField] private ChessBoardCommandExecutor chessBoardCommandExecutor;
        
        #endregion
        
        #region Unity Methods

        private void Update()
        {
            ChessBoardCommandArgs args = new ChessBoardCommandArgs(chessBoard);
            chessBoardCommandExecutor.UpdateCommands(args);
        }
        
        #endregion
    }
}