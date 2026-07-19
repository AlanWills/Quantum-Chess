using Celeste.Core.Interfaces;
using Celeste.Lua;
using QC.Match.Objects;
using QC.Match.Players;

namespace QC.Match.Commands
{
    public readonly struct ChessBoardCommandArgs
    {
        public readonly MatchState MatchState;
        public readonly ChessBoardCommandExecutor CommandExecutor; 
        public readonly ChessBoard ChessBoard;
        public readonly ChessBoardObjectSelection Selection;
        public readonly ILuaRuntime LuaRuntime;
        public readonly IVisualAnimationSystem VisualAnimationSystem;
        public readonly IRNG RNG;

        public ChessBoardCommandArgs(
            MatchState matchState,
            ChessBoardCommandExecutor commandExecutor,
            ChessBoard chessBoard,
            ChessBoardObjectSelection selection,
            ILuaRuntime luaRuntime,
            IVisualAnimationSystem visualAnimationSystem,
            IRNG rng)
        {
            MatchState = matchState;
            CommandExecutor = commandExecutor;
            ChessBoard = chessBoard;
            Selection = selection;
            LuaRuntime = luaRuntime;
            VisualAnimationSystem = visualAnimationSystem;
            RNG = rng;
        }
    }

    public static class ChessBoardCommandArgsExtensions
    {
        public static ITurnStrategy.Args ToTurnStrategyArgs(this ChessBoardCommandArgs args, int playerIndex)
        {
            return new ITurnStrategy.Args(
                playerIndex,
                args.MatchState,
                args.ChessBoard,
                args.CommandExecutor,
                args.LuaRuntime,
                args.RNG);
        }

        public static IWinCondition.Args ToWinConditionArgs(this ChessBoardCommandArgs args)
        {
            return new IWinCondition.Args(
                args.ChessBoard,
                args.LuaRuntime);
        }
    }
}