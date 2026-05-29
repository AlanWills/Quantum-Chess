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
        public List<ChessBoardObjectDTO> objects = new();
        public List<SerializableVector2Int> boardPositions = new();

        public ChessBoardDTO(ChessBoard chessBoard)
        {
            foreach (ChessBoardObjectInstance chessBoardObjectInstance in chessBoard.Objects)
            {
                objects.Add(new ChessBoardObjectDTO(chessBoardObjectInstance));
            }

            foreach (Vector2Int boardPosition in chessBoard.BoardPositions)
            {
                boardPositions.Add(boardPosition.ToSerializable());
            }
        }
    }
}