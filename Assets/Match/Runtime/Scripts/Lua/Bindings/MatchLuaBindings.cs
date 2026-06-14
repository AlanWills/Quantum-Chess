using Celeste;
using Celeste.Lua;
using QC.Match.Catalogue;
using QC.Match.Lua;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match
{
    public class MatchLuaBindings : MonoBehaviour, ILuaBindings
    {
        #region Properties and Fields

        [SerializeField] private MatchState matchState;
        [SerializeField] private MatchScenarioCatalogue matchScenarioCatalogue;
        [SerializeField] private ChessBoard chessBoard;
        [SerializeField] private ChessBoardObjectSelection chessBoardObjectSelection;
        [SerializeField] private ChessBoardCommandExecutor chessBoardCommandExecutor;
        
        #endregion
        
        public void Bind(LuaRuntime luaRuntime)
        {
            luaRuntime.SetEnvironmentVariable(MatchState.kLuaName, matchState);
            luaRuntime.SetEnvironmentVariable(MatchScenarioCatalogue.kLuaName, matchScenarioCatalogue);
            luaRuntime.SetEnvironmentVariable(ChessBoard.kLuaName, chessBoard);
            luaRuntime.SetEnvironmentVariable(ChessBoardObjectSelection.kLuaName, chessBoardObjectSelection);
            luaRuntime.SetEnvironmentVariable(ChessBoardCommandExecutor.kLuaName, chessBoardCommandExecutor);
            luaRuntime.OpenLibrary(MatchCommandsLibrary.Instance);
            
            UnityEngine.Debug.Log("Match Lua Bindings set up successfully!", CelesteLog.Lua);
        }

        public void Unbind(LuaRuntime luaRuntime)
        {
            luaRuntime.ClearEnvironmentVariable(MatchState.kLuaName);
            luaRuntime.ClearEnvironmentVariable(MatchScenarioCatalogue.kLuaName);
            luaRuntime.ClearEnvironmentVariable(ChessBoard.kLuaName);
            luaRuntime.ClearEnvironmentVariable(ChessBoardObjectSelection.kLuaName);
            luaRuntime.ClearEnvironmentVariable(ChessBoardCommandExecutor.kLuaName);
        }
    }
}
