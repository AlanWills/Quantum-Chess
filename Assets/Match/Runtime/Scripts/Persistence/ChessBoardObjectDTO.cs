using System;
using System.Collections.Generic;
using Celeste.Components.Persistence;
using QC.Match.Objects;

namespace QC.Match.Persistence
{
    [Serializable]
    public class ChessBoardObjectDTO
    {
        public int guid;
        public List<ComponentDTO> components = new();
        
        public ChessBoardObjectDTO(BoardObjectInstance boardObjectInstance)
        {
            guid = boardObjectInstance.Guid;
            
            foreach (var component in boardObjectInstance.Components)
            {
                components.Add(ComponentDTO.From(component));
            }
        }
    }
}