using System;
using System.Collections.Generic;
using Celeste.Persistence.Wrappers;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Persistence
{
    [Serializable]
    public class ChessBoardDTO : Celeste.Persistence.VersionedDTO
    {
        public List<ChessBoardTileDTO> tiles = new();
        public List<ChessBoardObjectDTO> objects = new();

        public ChessBoardDTO(ChessBoard chessBoard)
        {
            foreach (ChessBoardTileInstance tile in chessBoard.Tiles)
            {
                tiles.Add(new ChessBoardTileDTO(tile));
            }
            
            foreach (ChessBoardObjectInstance chessBoardObjectInstance in chessBoard.Objects)
            {
                objects.Add(new ChessBoardObjectDTO(chessBoardObjectInstance));
            }
        }
    }
}