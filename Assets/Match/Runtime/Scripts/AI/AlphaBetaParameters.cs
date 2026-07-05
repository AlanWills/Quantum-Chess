using System;
using System.Collections.Generic;
using Celeste.Lua;
using Lua;
using Lua.Unity;
using UnityEngine;

namespace QC.Match.AI
{
    [Serializable]
    public struct AlphaBetaParameters
    {
        #region Coordinate Importance

        [Serializable]
        private struct CoordinateImportance
        {
            public Vector2Int position;
            public int importance;
        }
        
        #endregion
        
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
        [SerializeField] private List<CoordinateImportance> coordinateImportanceOverrides;
        
        #endregion

        public int GetImportanceOverrideForPosition(Vector2Int position)
        {
            int indexOfPosition = coordinateImportanceOverrides.FindIndex(x => x.position == position);
            return indexOfPosition >= 0 ? coordinateImportanceOverrides[indexOfPosition].importance : 0;
        }

        public static AlphaBetaParameters FromTableOverride(AlphaBetaParameters parameters, LuaTable overrideTable)
        {
            AlphaBetaParameters data = parameters;
            data.depth = overrideTable.TryGet("depth", data.depth);
            data.minThinkingTimeMilliseconds = overrideTable.TryGet("minThinkingTime", data.minThinkingTimeMilliseconds);

            if (overrideTable.TryGetTable("coordinateImportanceOverrides", out LuaTable importanceOverridesTable) &&
                importanceOverridesTable.ArrayLength > 0)
            {
                foreach (var v in importanceOverridesTable)
                {
                    // Need to override value if it already exists in the array, otherwise add it
                    if (!v.Value.IsTable())
                    {
                        UnityEngine.Debug.LogAssertion($"Failed to parse coordinate importance override ({v.Key}) as the value is not a table.");
                        continue;
                    }

                    LuaTable table = v.Value.ReadTable();
                    CoordinateImportance coordinateImportance = new();

                    if (!table.TryGetVector2Int("pos", out coordinateImportance.position))
                    {
                        UnityEngine.Debug.LogAssertion($"Failed to parse 'pos' as a {nameof(Vector2Int)}.");
                        continue;
                    }

                    if (!table.TryGet("importance", out coordinateImportance.importance))
                    {
                        UnityEngine.Debug.LogAssertion($"Failed to parse 'importance' as a {nameof(Vector2Int)}.");
                        continue;
                    }

                    int indexOfExistingPosition =
                        data.coordinateImportanceOverrides.FindIndex(x => x.position == coordinateImportance.position); 
                    if (indexOfExistingPosition >= 0)
                    {
                        data.coordinateImportanceOverrides[indexOfExistingPosition] = coordinateImportance;
                    }
                    else
                    {
                        data.coordinateImportanceOverrides.Add(coordinateImportance);
                    }
                }
            }

            return data;
        }
    }

}