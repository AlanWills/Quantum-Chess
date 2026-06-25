using Celeste.Lua;
using Lua;

namespace QC.Match.Players
{
    public static class TurnStrategyFactory
    {
        public enum Type
        {
            Manual,
            AI,
            Custom
        }

        public static ITurnStrategy Create(Type turnStrategyType, LuaTable luaTable)
        {
            switch (turnStrategyType)
            {
                case Type.Manual:
                    return new ManualTurnStrategy();
                
                case Type.AI:
                    if (luaTable.TryGetTable("turnStrategyData", out LuaTable strategyDataTable))
                    {
                        AITurnStrategyData data = AITurnStrategyData.FromTable(strategyDataTable);
                        return new AITurnStrategy(data);
                    }
                    return new AITurnStrategy(AITurnStrategyData.Default);

                case Type.Custom:
                {
                    if (luaTable.TryGetFunction("onTurnBegin", out var onTurnBeginFunc) &&
                        luaTable.TryGetFunction("onTurnEnd", out var onTurnEndFunc))
                    {
                        return new LuaFunctionTurnStrategy(onTurnBeginFunc, onTurnEndFunc);
                    }
                    
                    UnityEngine.Debug.LogAssertion($"Failed to create {nameof(Type.Custom)} turn strategy as the factory was unable to detect the 'onTurnBegin' and 'onTurnEnd' functions.");
                    return new NoTurnStrategy();
                }
                
                default:
                    UnityEngine.Debug.LogAssertion($"Unhandled turn strategy type {turnStrategyType}...");
                    return new NoTurnStrategy();
            }
        }
    }
}