using Celeste.Lua;
using QC.Match.Record;
using UnityEngine;

namespace QC.Match
{
    public class MatchLuaBindings : MonoBehaviour, ILuaBindings
    {
        #region Properties and Fields
        
        [SerializeField] private ChessBoard chessBoard;
        
        #endregion
        
        public void Bind(LuaRuntime luaRuntime)
        {
            luaRuntime.SetEnvironmentVariable(ChessBoard.kLuaName, chessBoard);
        }

        public void Unbind(LuaRuntime luaRuntime)
        {
            luaRuntime.ClearEnvironmentVariable(ChessBoard.kLuaName);
        }
    }
}
