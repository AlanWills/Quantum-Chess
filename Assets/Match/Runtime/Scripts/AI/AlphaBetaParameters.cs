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

        [SerializeField] private int depth;
        
        #endregion

        public static AlphaBetaParameters FromTableOverride(AlphaBetaParameters parameters, LuaTable overrideTable)
        {
            AlphaBetaParameters data = parameters;
            overrideTable.TryGet("depth", out data.depth);

            return data;
        }
    }

}