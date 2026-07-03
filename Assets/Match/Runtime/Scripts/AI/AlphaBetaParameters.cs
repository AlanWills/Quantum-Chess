using System;
using Celeste.Lua;
using Lua;
using UnityEngine;

namespace QC.Match.AI
{
    [Serializable]
    public struct AlphaBetaParameters
    {
        #region Properties and Fields

        public int Depth
        {
            get => depth;
            set => depth = value;
        }

        public int MinThinkingTime
        {
            get => minThinkingTimeMilliseconds;
            set => minThinkingTimeMilliseconds = value;
        }

        [SerializeField] private int depth;
        [SerializeField] private int minThinkingTimeMilliseconds;
        
        #endregion

        public static AlphaBetaParameters FromTableOverride(AlphaBetaParameters parameters, LuaTable overrideTable)
        {
            AlphaBetaParameters data = parameters;
            data.depth = overrideTable.TryGet("depth", data.depth);
            data.minThinkingTimeMilliseconds = overrideTable.TryGet("minThinkingTime", data.minThinkingTimeMilliseconds);

            return data;
        }
    }

}