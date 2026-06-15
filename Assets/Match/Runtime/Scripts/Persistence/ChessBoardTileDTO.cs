using System;
using Celeste.Persistence.Wrappers;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Persistence
{
    [Serializable]
    public class ChessBoardTileDTO
    {
        public int guid;
        public SerializableVector2Int position;

        public ChessBoardTileDTO(ChessBoardTileInstance instance)
        {
            guid = instance.Guid;
            position = new SerializableVector2Int(instance.Position);
        }
    }
}