using Celeste.Lua;
using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using QC.Match.Players;

namespace QC.Match
{
    public interface IWinCondition
    {
        public struct Args
        {
            public readonly MatchState MatchState;
            public readonly ChessBoard ChessBoard;
            public readonly ILuaRuntime LuaRuntime;

            public Args(
                MatchState matchState,
                ChessBoard chessBoard,
                ILuaRuntime luaRuntime)
            {
                MatchState = matchState;
                ChessBoard = chessBoard;
                LuaRuntime = luaRuntime;
            }
        }
        
        UniTask<bool> CalculateIfWinConditionIsMet(Args args);
    }
}