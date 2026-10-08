using System;
using System.Collections.Generic;
using Celeste.Persistence.Wrappers;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Persistence
{
    [Serializable]
    public class BoardDTO : Celeste.Persistence.VersionedDTO
    {
        public List<BoardTileDTO> tiles = new();
        public List<ChessBoardObjectDTO> objects = new();

        public BoardDTO(Board board)
        {
            foreach (BoardTileInstance tile in board.Tiles)
            {
                tiles.Add(new BoardTileDTO(tile));
            }
            
            foreach (BoardObjectInstance chessBoardObjectInstance in board.Objects)
            {
                objects.Add(new ChessBoardObjectDTO(chessBoardObjectInstance));
            }
        }
    }
}