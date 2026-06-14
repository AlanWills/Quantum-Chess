using Celeste.Lua;
using Cysharp.Threading.Tasks;
using Lua;

namespace QC.Match.Players
{
    public class LuaFunctionTurnStrategy : ITurnStrategy
    {
        private readonly LuaFunction TurnBeginFunction;
        private readonly LuaFunction TurnEndFunction;
        
        public LuaFunctionTurnStrategy(
            LuaFunction turnBeginFunction,
            LuaFunction turnEndFunction)
        {
            UnityEngine.Debug.Assert(turnBeginFunction != null, $"Null {nameof(turnBeginFunction)} passed into {nameof(LuaFunctionWinCondition)}.");
            TurnBeginFunction = turnBeginFunction;
            TurnEndFunction = turnEndFunction;
        }
        
        public async UniTask OnTurnBegin(ITurnStrategy.Args args)
        {
            await args.LuaRuntime.ExecuteFunctionAsync(TurnBeginFunction);
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