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
        
        public ChessBoardObjectDTO(ChessBoardObjectInstance chessBoardObjectInstance)
        {
            guid = chessBoardObjectInstance.Guid;
            
            foreach (var component in chessBoardObjectInstance.Components)
            {
                components.Add(ComponentDTO.From(component));
            }
        }
    }
}