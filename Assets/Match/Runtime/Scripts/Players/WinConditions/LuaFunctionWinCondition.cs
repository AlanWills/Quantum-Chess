using Celeste.Lua;
using Cysharp.Threading.Tasks;
using Lua;

namespace QC.Match.Players
{
    public class LuaFunctionWinCondition : IWinCondition
    {
        private readonly LuaFunction WinConditionFunction;
        
        public LuaFunctionWinCondition(LuaFunction winConditionFunction)
        {
            UnityEngine.Debug.Assert(winConditionFunction != null, $"Null {nameof(winConditionFunction)} passed into {nameof(LuaFunctionWinCondition)}.");
            WinConditionFunction = winConditionFunction;
        }

        public async UniTask<bool> CalculateIfWinConditionIsMet(IWinCondition.Args args)
        {
            LuaValue[] luaValues = await args.LuaRuntime.ExecuteFunctionAsync(WinConditionFunction);
            return luaValues?.Length == 1 && luaValues[0].ToBoolean();
        }
    }
}