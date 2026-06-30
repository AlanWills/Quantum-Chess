using Celeste.Core.Interfaces;
using Celeste.Lua;
using QC.Match.Objects;

namespace QC.Match.Commands
{
    public readonly struct ChessBoardCommandArgs
    {
        public readonly MatchState MatchState;
        public readonly ChessBoardCommandExecutor CommandExecutor; 
        public readonly ChessBoard ChessBoard;
        public readonly ChessBoardObjectSelection Selection;
        public readonly ILuaRuntime LuaRuntime;
        public readonly ITimeProvider TimeProvider;

        public ChessBoardCommandArgs(
            MatchState matchState,
            ChessBoardCommandExecutor commandExecutor,
            ChessBoard chessBoard,
            ChessBoardObjectSelection selection,
            ILuaRuntime luaRuntime,
            ITimeProvider timeProvider)
        {
            MatchState = matchState;
            CommandExecutor = commandExecutor;
            ChessBoard = chessBoard;
            Selection = selection;
            LuaRuntime = luaRuntime;
            TimeProvider = timeProvider;
        }
    }
}