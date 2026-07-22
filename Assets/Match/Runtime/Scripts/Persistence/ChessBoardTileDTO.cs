using System;
using System.Collections.Generic;
using Celeste.Components.Persistence;
using Celeste.Persistence.Wrappers;
using QC.Match.Objects;

namespace QC.Match.Persistence
{
    [Serializable]
    public class ChessBoardTileDTO
    {
        public int guid;
        public SerializableVector2Int position;
        public List<ComponentDTO> components = new();

        public ChessBoardTileDTO(BoardTileInstance instance)
        {
            guid = instance.Guid;
            position = new SerializableVector2Int(instance.Position);
            
            foreach (var component in instance.Components)
            {
                components.Add(ComponentDTO.From(component));
            }
        }
    }
}