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
        [SerializeField] private CommandExecutor commandExecutor;
        
        #endregion
        
        public void Bind(LuaRuntime luaRuntime)
        {
            luaRuntime.SetEnvironmentVariable(ChessBoard.kLuaName, chessBoard);
            luaRuntime.SetEnvironmentVariable(CommandExecutor.kLuaName, commandExecutor);
            luaRuntime.OpenLibrary(ChessBoardCommandsLibrary.Instance);
        }

        public void Unbind(LuaRuntime luaRuntime)
        {
            luaRuntime.ClearEnvironmentVariable(ChessBoard.kLuaName);
            luaRuntime.ClearEnvironmentVariable(CommandExecutor.kLuaName);
        }
    }
}
