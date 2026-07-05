using System;
using System.Collections.Generic;
using Celeste.Core.Interfaces;
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
        
        #region Piece Value

        [Serializable]
        private struct PieceValue
        {
            public ChessPieceType pieceType;
            public int value;
            public Vector2Int variance;
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
        [SerializeField] private List<PieceValue> pieceValueOverrides;
        
        #endregion

        public int GetImportanceOverrideForPosition(Vector2Int position)
        {
            int indexOfPosition = coordinateImportanceOverrides.FindIndex(x => x.position == position);
            return indexOfPosition >= 0 ? coordinateImportanceOverrides[indexOfPosition].importance : 0;
        }

        public int GetValueOverrideForPiece(ChessPieceType chessPieceType, IRNG rng)
        {
            int indexOfType = pieceValueOverrides.FindIndex(x => x.pieceType == chessPieceType);
            if (indexOfType < 0)
            {
                return 0;
            }
            
            // Handle variance here
            
            return indexOfType >= 0 ? pieceValueOverrides[indexOfType].value : 0;
        }

        public static AlphaBetaParameters FromTableOverride(AlphaBetaParameters parameters, LuaTable overrideTable)
        {
            AlphaBetaParameters data = parameters;
            data.depth = overrideTable.TryGet("depth", data.depth);
            data.minThinkingTimeMilliseconds = overrideTable.TryGet("minThinkingTime", data.minThinkingTimeMilliseconds);

            // Coordinate Importance Overrides
            {
                if (overrideTable.TryGetTable("coordinateImportanceOverrides", out LuaTable importanceOverridesTable) &&
                    importanceOverridesTable.ArrayLength > 0)
                {
                    foreach (var v in importanceOverridesTable)
                    {
                        if (!v.Value.IsTable())
                        {
                            UnityEngine.Debug.LogAssertion(
                                $"Failed to parse coordinate importance override ({v.Key}) as the value is not a table.");
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
                            data.coordinateImportanceOverrides.FindIndex(x =>
                                x.position == coordinateImportance.position);
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
            }

            // Piece Value Overrides
            {
                if (overrideTable.TryGetTable("pieceValueOverrides", out LuaTable pieceValueOverrides) &&
                    pieceValueOverrides.ArrayLength > 0)
                {
                    foreach (var v in pieceValueOverrides)
                    {
                        if (!v.Value.IsTable())
                        {
                            UnityEngine.Debug.LogAssertion(
                                $"Failed to parse piece value override ({v.Key}) as the value is not a table.");
                            continue;
                        }

                        LuaTable table = v.Value.ReadTable();
                        PieceValue pieceValue = new();

                        if (!table.TryGetEnum("pieceType", out pieceValue.pieceType))
                        {
                            UnityEngine.Debug.LogAssertion($"Failed to parse 'pieceType' as a {nameof(ChessPieceType)}.");
                            continue;
                        }

                        if (!table.TryGet("value", out pieceValue.value))
                        {
                            UnityEngine.Debug.LogAssertion($"Failed to parse 'value' as an int.");
                            continue;
                        }

                        int indexOfExistingPosition = data.pieceValueOverrides.FindIndex(x => x.pieceType == pieceValue.pieceType);
                        if (indexOfExistingPosition >= 0)
                        {
                            data.pieceValueOverrides[indexOfExistingPosition] = pieceValue;
                        }
                        else
                        {
                            data.pieceValueOverrides.Add(pieceValue);
                        }
                    }
                }
            }

            return data;
        }
    }

}