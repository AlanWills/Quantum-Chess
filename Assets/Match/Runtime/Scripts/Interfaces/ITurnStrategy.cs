using Celeste.Lua;
using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using QC.Match.Players;

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

            public Args(
                MatchState matchState,
                ChessBoard chessBoard,
                ChessBoardCommandExecutor commandExecutor,
                ILuaRuntime luaRuntime)
            {
                MatchState = matchState;
                ChessBoard = chessBoard;
                CommandExecutor = commandExecutor;
                LuaRuntime = luaRuntime;
            }
        }

        UniTask CalculateTurnStrategy(Args args);
    }
}