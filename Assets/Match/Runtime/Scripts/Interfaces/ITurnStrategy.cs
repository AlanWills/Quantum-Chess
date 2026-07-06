using Celeste.Core.Interfaces;
using Celeste.Lua;
using Cysharp.Threading.Tasks;
using QC.Match.Objects;

namespace QC.Match
{
    public interface ITurnStrategy
    {
        public struct Args
        {
            public readonly MatchState MatchState;
            public readonly ChessBoard ChessBoard;
            public readonly ChessBoardCommandExecutor CommandExecutor;
            public readonly ILuaRuntime LuaRuntime;
            public readonly IRNG RNG;

            public Args(
                MatchState matchState,
                ChessBoard chessBoard,
                ChessBoardCommandExecutor commandExecutor,
                ILuaRuntime luaRuntime,
                IRNG rng)
            {
                MatchState = matchState;
                ChessBoard = chessBoard;
                CommandExecutor = commandExecutor;
                LuaRuntime = luaRuntime;
                RNG = rng;
            }
        }

        UniTask OnTurnBegin(Args args);
        UniTask OnTurnEnd(Args args);
    }
}