using Celeste.Core.Interfaces;
using Celeste.Lua;
using QC.Match.Objects;
using QC.Match.Players;

namespace QC.Match.Commands
{
    public readonly struct BoardCommandArgs
    {
        public readonly MatchState MatchState;
        public readonly BoardCommandExecutor CommandExecutor; 
        public readonly Board Board;
        public readonly BoardObjectSelection Selection;
        public readonly ILuaRuntime LuaRuntime;
        public readonly IVisualAnimationSystem VisualAnimationSystem;
        public readonly IRNG RNG;

        public BoardCommandArgs(
            MatchState matchState,
            BoardCommandExecutor commandExecutor,
            Board board,
            BoardObjectSelection selection,
            ILuaRuntime luaRuntime,
            IVisualAnimationSystem visualAnimationSystem,
            IRNG rng)
        {
            MatchState = matchState;
            CommandExecutor = commandExecutor;
            Board = board;
            Selection = selection;
            LuaRuntime = luaRuntime;
            VisualAnimationSystem = visualAnimationSystem;
            RNG = rng;
        }
    }

    public static class ChessBoardCommandArgsExtensions
    {
        public static ITurnStrategy.Args ToTurnStrategyArgs(this BoardCommandArgs args, int playerIndex)
        {
            return new ITurnStrategy.Args(
                playerIndex,
                args.MatchState,
                args.MatchState,
                args.Board,
                args.CommandExecutor,
                args.LuaRuntime,
                args.RNG);
        }

        public static IWinCondition.Args ToWinConditionArgs(this BoardCommandArgs args)
        {
            return new IWinCondition.Args(
                args.Board,
                args.MatchState,
                args.LuaRuntime);
        }
    }
}