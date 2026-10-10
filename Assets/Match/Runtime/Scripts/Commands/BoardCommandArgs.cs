using Celeste.Components.Catalogue;
using Celeste.Core.Interfaces;
using Celeste.Lua;
using QC.Match.Objects;

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
        public readonly IPromotionSystem PromotionSystem;
        public readonly IRNG RNG;
        public readonly ComponentCatalogue ComponentCatalogue;

        public BoardCommandArgs(
            MatchState matchState,
            BoardCommandExecutor commandExecutor,
            Board board,
            BoardObjectSelection selection,
            ILuaRuntime luaRuntime,
            IVisualAnimationSystem visualAnimationSystem,
            IPromotionSystem promotionSystem,
            IRNG rng,
            ComponentCatalogue componentCatalogue)
        {
            MatchState = matchState;
            CommandExecutor = commandExecutor;
            Board = board;
            Selection = selection;
            LuaRuntime = luaRuntime;
            VisualAnimationSystem = visualAnimationSystem;
            PromotionSystem = promotionSystem;
            RNG = rng;
            ComponentCatalogue = componentCatalogue;
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

        public static IWinCondition.Args ToWinConditionArgs(this BoardCommandArgs args, int playerIndex)
        {
            return new IWinCondition.Args(
                playerIndex,
                args.Board,
                args.MatchState,
                args.LuaRuntime);
        }
    }
}