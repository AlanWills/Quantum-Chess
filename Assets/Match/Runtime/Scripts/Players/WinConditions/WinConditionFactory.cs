using Celeste.Lua;
using Lua;

namespace QC.Match.Players
{
    public static class WinConditionFactory
    {
        public enum Type
        {
            Checkmate,
            Custom
        }
        
        public static IWinCondition Create(
            Type winConditionType, 
            LuaTable luaTable)
        {
            switch (winConditionType)
            {
                case Type.Checkmate:
                {
                    if (luaTable.TryGetEnum("winConditionData", out PieceColour oppositionColour))
                    {
                        return new CheckmateWinCondition(oppositionColour);
                    }

                    UnityEngine.Debug.LogAssertion($"Failed to create {nameof(Type.Checkmate)} win condition.  Missing Opposition Colour.");
                    return new NoWinCondition();
                }

                case Type.Custom:
                {
                    if (luaTable.TryGetFunction("winConditionFunc", out LuaFunction winConditionFunc))
                    {
                        return new LuaFunctionWinCondition(winConditionFunc);
                    }

                    UnityEngine.Debug.LogAssertion($"Failed to create {nameof(Type.Custom)} win condition.  Missing Win Condition Func.");
                    return new NoWinCondition();
                }
                
                default:
                    UnityEngine.Debug.LogAssertion($"Unhandled Win Condition Type {winConditionType}.");
                    return new NoWinCondition();
            }
        }
    }
}