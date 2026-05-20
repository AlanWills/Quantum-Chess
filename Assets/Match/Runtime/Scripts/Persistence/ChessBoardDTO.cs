using System;
using System.Collections.Generic;
using QC.Match.Objects;
using QC.Match.Record;

namespace QC.Match.Persistence
{
    [Serializable]
    public class ChessBoardDTO : Celeste.Persistence.VersionedDTO
    {
        public List<ChessBoardObjectDTO> objects = new();

        public ChessBoardDTO(ChessBoard chessBoard)
        {
            foreach (ChessBoardObjectInstance chessBoardObjectInstance in chessBoard.Objects)
            {
                objects.Add(new ChessBoardObjectDTO(chessBoardObjectInstance));
            }
        }
    }
}