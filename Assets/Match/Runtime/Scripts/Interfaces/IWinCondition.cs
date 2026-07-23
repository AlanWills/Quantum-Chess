using Celeste.Lua;
using Cysharp.Threading.Tasks;
using QC.Match.Objects;

namespace QC.Match
{
    public interface IWinCondition
    {
        public readonly struct Args
        {
            public readonly Board Board;
            public readonly ILuaRuntime LuaRuntime;

            public Args(
                Board board,
                ILuaRuntime luaRuntime)
            {
                Board = board;
                LuaRuntime = luaRuntime;
            }
        }
        
        UniTask<bool> CalculateIfWinConditionIsMet(Args args);
    }
}