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
            public readonly Board Board;
            public readonly IPlayerActionMaskLookup PlayerActionMaskLookup;
            public readonly ILuaRuntime LuaRuntime;

            public Args(
                Board board,
                IPlayerActionMaskLookup playerActionMaskLookup,
                ILuaRuntime luaRuntime)
            {
                Board = board;
                PlayerActionMaskLookup = playerActionMaskLookup;
                LuaRuntime = luaRuntime;
            }
        }
        
        UniTask<bool> CalculateIfWinConditionIsMet(Args args);
    }
}