using Celeste.Lua;
using QC.Match.Lua;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match
{
    public class MatchLuaBindings : MonoBehaviour, ILuaBindings
    {
        #region Properties and Fields
        
        [SerializeField] private ChessBoard chessBoard;
        [SerializeField] private ChessBoardObjectSelection chessBoardObjectSelection;
        [SerializeField] private ChessBoardCommandExecutor chessBoardCommandExecutor;
        
        #endregion
        
        public void Bind(LuaRuntime luaRuntime)
        {
            luaRuntime.SetEnvironmentVariable(ChessBoard.kLuaName, chessBoard);
            luaRuntime.SetEnvironmentVariable(ChessBoardObjectSelection.kLuaName, chessBoardObjectSelection);
            luaRuntime.SetEnvironmentVariable(ChessBoardCommandExecutor.kLuaName, chessBoardCommandExecutor);
            luaRuntime.OpenLibrary(ChessBoardCommandsLibrary.Instance);
        }

        public void Unbind(LuaRuntime luaRuntime)
        {
            luaRuntime.ClearEnvironmentVariable(ChessBoard.kLuaName);
            luaRuntime.ClearEnvironmentVariable(ChessBoardObjectSelection.kLuaName);
            luaRuntime.ClearEnvironmentVariable(ChessBoardCommandExecutor.kLuaName);
        }
    }
}
