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

        public async UniTask OnTurn(ITurnStrategy.Args args, bool dryRun)
        {
            await args.LuaRuntime.ExecuteFunctionAsync(TurnActionFunction, dryRun);
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