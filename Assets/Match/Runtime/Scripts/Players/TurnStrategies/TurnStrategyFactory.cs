using Celeste.Lua;
using Lua;
using QC.Match.AI;

namespace QC.Match.Players
{
    public static class TurnStrategyFactory
    {
        public enum Type
        {
            Manual,
            AlphaBeta,
            Random,
            Custom
        }

        public enum AIDifficulty
        {
            Easy,
            Medium,
            Hard,
            Expert
        }

        public static ITurnStrategy Create(Type turnStrategyType, AIStrategyDefaultParameters defaultParameters, LuaTable luaTable)
        {
            switch (turnStrategyType)
            {
                case Type.Manual:
                    return new ManualTurnStrategy();
                
                case Type.AlphaBeta:
                    if (luaTable.TryGetTable("turnStrategyData", out LuaTable strategyDataTable) &&
                        strategyDataTable.TryGetEnum("difficulty", out AIDifficulty difficulty))
                    {
                        AlphaBetaParameters parameters = defaultParameters.AlphaBetaParametersFromAIDifficulty(difficulty);
                        parameters = AlphaBetaParameters.FromTableOverride(parameters, strategyDataTable);
                        return new AlphaBetaTurnStrategy(parameters);
                    }
                    return new AlphaBetaTurnStrategy(defaultParameters.AlphaBeta_Medium);

                case Type.Random:
                    return new RandomActionTurnStrategy();

                case Type.Custom:
                {
                    if (luaTable.TryGetFunction("onTurnBegin", out var onTurnBeginFunc) &&
                        luaTable.TryGetFunction("turnAction", out var turnActionFunc) &&
                        luaTable.TryGetFunction("onTurnEnd", out var onTurnEndFunc))
                    {
                        return new LuaFunctionTurnStrategy(onTurnBeginFunc, turnActionFunc, onTurnEndFunc);
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