using Celeste.Lua;
using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using QC.Match.Players;

namespace QC.Match
{
    public interface IWinCondition
    {
        public readonly struct Args
        {
            public readonly ChessBoard ChessBoard;
            public readonly ILuaRuntime LuaRuntime;

            public Args(
                ChessBoard chessBoard,
                ILuaRuntime luaRuntime)
            {
                ChessBoard = chessBoard;
                LuaRuntime = luaRuntime;
            }
        }
        
        UniTask<bool> CalculateIfWinConditionIsMet(Args args);
    }
}