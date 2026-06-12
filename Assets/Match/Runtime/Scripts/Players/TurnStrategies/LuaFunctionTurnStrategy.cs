using Celeste.Lua;
using Cysharp.Threading.Tasks;
using Lua;

namespace QC.Match.Players
{
    public class LuaFunctionTurnStrategy : ITurnStrategy
    {
        private readonly LuaFunction TurnStrategyFunction;
        
        public LuaFunctionTurnStrategy(LuaFunction turnStrategyFunction)
        {
            UnityEngine.Debug.Assert(turnStrategyFunction != null, $"Null {nameof(turnStrategyFunction)} passed into {nameof(LuaFunctionWinCondition)}.");
            TurnStrategyFunction = turnStrategyFunction;
        }
        
        public async UniTask CalculateTurnStrategy(ITurnStrategy.Args args)
        {
            await args.LuaRuntime.ExecuteFunctionAsync(TurnStrategyFunction);
        }
    }
}