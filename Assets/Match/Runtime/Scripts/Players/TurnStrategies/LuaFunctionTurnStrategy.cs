using Celeste.Lua;
using Cysharp.Threading.Tasks;
using Lua;
using QC.Match.Commands;

namespace QC.Match.Players
{
    public class LuaFunctionTurnStrategy : ITurnStrategy
    {
        private readonly LuaFunction TurnBeginFunction;
        private readonly LuaFunction TurnActionFunction;
        private readonly LuaFunction TurnEndFunction;
        
        public LuaFunctionTurnStrategy(
            LuaFunction turnBeginFunction,
            LuaFunction turnActionFunction,
            LuaFunction turnEndFunction)
        {
            UnityEngine.Debug.Assert(turnBeginFunction != null, $"Null {nameof(turnBeginFunction)} passed into {nameof(LuaFunctionWinCondition)}.");
            TurnBeginFunction = turnBeginFunction;
            TurnActionFunction = turnActionFunction;
            TurnEndFunction = turnEndFunction;
        }
        
        public async UniTask OnTurnBegin(ITurnStrategy.Args args)
        {
            await args.LuaRuntime.ExecuteFunctionAsync(TurnBeginFunction);
        }

        public async UniTask<ICommand<ChessBoardCommandArgs>> CalculateNextAction(ITurnStrategy.Args args)
        {
            // TODO: This won't work right now - how do we provide people a way of creating actions in Lua without
            // adding to the CommandExecutor global state.  Can we pass a fake one into this function and then
            // extract out the command they've supplied it to return?
            await args.LuaRuntime.ExecuteFunctionAsync(TurnBeginFunction);
            return null;
        }

        public UniTask DoTurn(ITurnStrategy.Args args)
        {
            // TODO: This won't work right now - probably need to treat the Lua function as an update loop or something and call
            // it every frame until it returns a value or something?
            return UniTask.CompletedTask;
        }

        public async UniTask OnTurnEnd(ITurnStrategy.Args args)
        {
            if (TurnEndFunction != null)
            {
                await args.LuaRuntime.ExecuteFunctionAsync(TurnEndFunction);
            }
        }
    }
}