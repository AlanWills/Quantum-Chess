using System;
using Celeste.Tools.Attributes.GUI;
using UnityEngine;

namespace QC.Match.Actions
{
    [Serializable]
    public struct ActionPattern
    {
        public RangeType range;
        [ShowIfEnum(nameof(range), (int)RangeType.Finite)] public int distanceIfNotMoved;
        [ShowIfEnum(nameof(range), (int)RangeType.Finite)] public int distanceIfMoved;
        public Vector2Int pattern;
        public bool relativeToVerticalDirection;
        public bool mustBeSafe;
    }
}