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
        [SerializeField] private Board board;
        [SerializeField] private BoardObjectSelection boardObjectSelection;
        [SerializeField] private BoardCommandExecutor boardCommandExecutor;
        
        #endregion
        
        public void Bind(LuaRuntime luaRuntime)
        {
            luaRuntime.SetEnvironmentVariable(MatchState.kLuaName, matchState);
            luaRuntime.SetEnvironmentVariable(MatchScenarioCatalogue.kLuaName, matchScenarioCatalogue);
            luaRuntime.SetEnvironmentVariable(Board.kLuaName, board);
            luaRuntime.SetEnvironmentVariable(BoardObjectSelection.kLuaName, boardObjectSelection);
            luaRuntime.SetEnvironmentVariable(BoardCommandExecutor.kLuaName, boardCommandExecutor);
            luaRuntime.OpenLibrary(MatchCommandsLibrary.Instance);
            
            UnityEngine.Debug.Log("Match Lua Bindings set up successfully!", CelesteLog.Lua);
        }

        public void Unbind(LuaRuntime luaRuntime)
        {
            luaRuntime.ClearEnvironmentVariable(MatchState.kLuaName);
            luaRuntime.ClearEnvironmentVariable(MatchScenarioCatalogue.kLuaName);
            luaRuntime.ClearEnvironmentVariable(Board.kLuaName);
            luaRuntime.ClearEnvironmentVariable(BoardObjectSelection.kLuaName);
            luaRuntime.ClearEnvironmentVariable(BoardCommandExecutor.kLuaName);
        }
    }
}
