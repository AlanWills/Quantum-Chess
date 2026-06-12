using Celeste.Lua;
using Lua;

namespace QC.Match.Players
{
    public static class TurnStrategyFactory
    {
        public enum Type
        {
            Manual,
            Custom
        }

        public static ITurnStrategy Create(Type turnStrategyType, LuaTable luaTable)
        {
            switch (turnStrategyType)
            {
                case Type.Manual:
                    return new ManualTurnStrategy();

                case Type.Custom:
                {
                    if (luaTable.TryGetFunction("turnStrategyFunc", out var turnStrategyFunc))
                    {
                        return new LuaFunctionTurnStrategy(turnStrategyFunc);
                    }
                    
                    UnityEngine.Debug.LogAssertion($"Failed to create {nameof(Type.Custom)} turn strategy as no 'turnStrategyFunc' was detected.");
                    return new NoTurnStrategy();
                }
                
                default:
                    UnityEngine.Debug.LogAssertion($"Unhandled turn strategy type {turnStrategyType}...");
                    return new NoTurnStrategy();
            }
        }
    }
}