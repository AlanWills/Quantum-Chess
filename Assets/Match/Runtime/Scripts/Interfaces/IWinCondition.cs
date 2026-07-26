using Celeste.Lua;
using Cysharp.Threading.Tasks;
using QC.Match.Objects;

namespace QC.Match
{
    public interface IWinCondition
    {
        public readonly struct Args
        {
            public readonly int PlayerIndex;
            public readonly Board Board;
            public readonly IBoardActionMaskLookup BoardActionMaskLookup;
            public readonly ILuaRuntime LuaRuntime;

            public Args(
                int playerIndex,
                Board board,
                IBoardActionMaskLookup boardActionMaskLookup,
                ILuaRuntime luaRuntime)
            {
                PlayerIndex = playerIndex;
                Board = board;
                BoardActionMaskLookup = boardActionMaskLookup;
                LuaRuntime = luaRuntime;
            }
        }
        
        UniTask<bool> CalculateIfWinConditionIsMet(Args args);
    }
}