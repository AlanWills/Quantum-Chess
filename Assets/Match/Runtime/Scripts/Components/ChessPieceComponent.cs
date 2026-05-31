using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Celeste.Components;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Components
{
    [DisplayName("Chess Piece Type")]
    public class ChessPieceComponent : ChessBoardObjectComponent, IChessPiece, IObjectActionProvider
    {
        #region Save Data

        [Serializable]
        public class SaveData : ComponentData
        {
            public ChessPieceColour currentPieceColour;
            public ChessPieceType currentPieceType;
        }
        
        #endregion
        
        #region Properties and Fields
        
        [SerializeField] private ChessPieceColour startingColour = ChessPieceColour.Unknown;
        [SerializeField] private ChessPieceType startingPieceType = ChessPieceTypeExtensions.Everything;
        
        #endregion

        public override ComponentData CreateData()
        {
            return new SaveData
            {
                currentPieceColour = startingColour,
                currentPieceType = startingPieceType
            };
        }

        public ChessPieceColour GetPieceColour(Instance instance)
        {
            return (instance.data as SaveData).currentPieceColour;
        }

        public void SetColour(Instance instance, ChessPieceColour colour)
        {
            SaveData saveData = instance.data as SaveData;
            if (saveData.currentPieceColour != colour)
            {
                saveData.currentPieceColour = colour;
                instance.events.ComponentDataChanged.Invoke();
            }
        }

        public ChessPieceType GetPieceType(Instance instance)
        {
            return (instance.data as SaveData).currentPieceType;
        }
        
        public void SetType(Instance instance, ChessPieceType type)
        {
            SaveData saveData = instance.data as SaveData;
            if (saveData.currentPieceType != type)
            {
                saveData.currentPieceType = type;
                instance.events.ComponentDataChanged.Invoke();
            }
        }

        public void CalculateValidMoves(
            Instance instance, 
            Vector2Int currentBoardPosition, 
            IReadOnlyCollection<Vector2Int> boardPositions, 
            IList<Vector2Int> validMoves)
        {
            SaveData saveData = instance.data as SaveData;
            switch (saveData.currentPieceType)
            {
               case ChessPieceType.None:
                   return;
               
               case ChessPieceType.Pawn:
                   AddIfBoardPositionExists(currentBoardPosition + new Vector2Int(0, 1), boardPositions, validMoves);
                   
                   return;
               
               case ChessPieceType.Knight:
                   AddIfBoardPositionExists(currentBoardPosition + new Vector2Int(-2, -1), boardPositions, validMoves);
                   AddIfBoardPositionExists(currentBoardPosition + new Vector2Int(-2, 1), boardPositions, validMoves);
                   AddIfBoardPositionExists(currentBoardPosition + new Vector2Int(2, -1), boardPositions, validMoves);
                   AddIfBoardPositionExists(currentBoardPosition + new Vector2Int(2, 1), boardPositions, validMoves);
                   AddIfBoardPositionExists(currentBoardPosition + new Vector2Int(-1, -2), boardPositions, validMoves);
                   AddIfBoardPositionExists(currentBoardPosition + new Vector2Int(-1, 2), boardPositions, validMoves);
                   AddIfBoardPositionExists(currentBoardPosition + new Vector2Int(1, -2), boardPositions, validMoves);
                   AddIfBoardPositionExists(currentBoardPosition + new Vector2Int(1, 2), boardPositions, validMoves);
                   
                   return;
               
               case ChessPieceType.Bishop:
                   FloodFillDirection(currentBoardPosition, new Vector2Int(-1, -1), boardPositions, validMoves);
                   FloodFillDirection(currentBoardPosition, new Vector2Int(-1, 1), boardPositions, validMoves);
                   FloodFillDirection(currentBoardPosition, new Vector2Int(1, -1), boardPositions, validMoves);
                   FloodFillDirection(currentBoardPosition, new Vector2Int(1, 1), boardPositions, validMoves);
                   
                   return;
               
               case ChessPieceType.Rook:
                   FloodFillDirection(currentBoardPosition, new Vector2Int(-1, 0), boardPositions, validMoves);
                   FloodFillDirection(currentBoardPosition, new Vector2Int(1, 0), boardPositions, validMoves);
                   FloodFillDirection(currentBoardPosition, new Vector2Int(0, -1), boardPositions, validMoves);
                   FloodFillDirection(currentBoardPosition, new Vector2Int(0, 1), boardPositions, validMoves);
                   
                   return;
               
               case ChessPieceType.Queen:
                   FloodFillDirection(currentBoardPosition, new Vector2Int(-1, -1), boardPositions, validMoves);
                   FloodFillDirection(currentBoardPosition, new Vector2Int(-1, 1), boardPositions, validMoves);
                   FloodFillDirection(currentBoardPosition, new Vector2Int(1, -1), boardPositions, validMoves);
                   FloodFillDirection(currentBoardPosition, new Vector2Int(1, 1), boardPositions, validMoves);
                   FloodFillDirection(currentBoardPosition, new Vector2Int(-1, 0), boardPositions, validMoves);
                   FloodFillDirection(currentBoardPosition, new Vector2Int(1, 0), boardPositions, validMoves);
                   FloodFillDirection(currentBoardPosition, new Vector2Int(0, -1), boardPositions, validMoves);
                   FloodFillDirection(currentBoardPosition, new Vector2Int(0, 1), boardPositions, validMoves);
                   
                   return;
               
               case ChessPieceType.King:
                   AddIfBoardPositionExists(currentBoardPosition + new Vector2Int(-1, -1), boardPositions, validMoves);
                   AddIfBoardPositionExists(currentBoardPosition + new Vector2Int(-1, 0), boardPositions, validMoves);
                   AddIfBoardPositionExists(currentBoardPosition + new Vector2Int(-1, 1), boardPositions, validMoves);
                   AddIfBoardPositionExists(currentBoardPosition + new Vector2Int(1, -1), boardPositions, validMoves);
                   AddIfBoardPositionExists(currentBoardPosition + new Vector2Int(1, 0), boardPositions, validMoves);
                   AddIfBoardPositionExists(currentBoardPosition + new Vector2Int(1, 1), boardPositions, validMoves);
                   AddIfBoardPositionExists(currentBoardPosition + new Vector2Int(0, -1), boardPositions, validMoves);
                   AddIfBoardPositionExists(currentBoardPosition + new Vector2Int(0, 1), boardPositions, validMoves);
                   return;
            }
        }

        public void CalculateValidAttacks(
            Instance instance, 
            Vector2Int currentBoardPosition, 
            IReadOnlyList<ChessBoardObjectInstance> otherObjects,
            IList<Vector2Int> validMoves)
        {
            SaveData saveData = instance.data as SaveData;
            switch (saveData.currentPieceType)
            {
                case ChessPieceType.None:
                    return;
               
                case ChessPieceType.Pawn:
                    return;
               
                case ChessPieceType.Knight:
                    return;
               
                case ChessPieceType.Bishop:
                    return;
               
                case ChessPieceType.Rook:
                    return;
               
                case ChessPieceType.Queen:
                    return;
               
                case ChessPieceType.King:
                    return;
            }
        }

        private static void AddIfBoardPositionExists(
            Vector2Int position,
            IReadOnlyCollection<Vector2Int> boardPositions,
            IList<Vector2Int> list)
        {
            if (boardPositions.Contains(position))
            {
                list.Add(position);
            }
        }

        private static void AddIfBoardPositionExists(
            Vector2Int position,
            IReadOnlyCollection<Vector2Int> boardPositions,
            ICollection<Vector2Int> collection)
        {
            if (boardPositions.Contains(position))
            {
                collection.Add(position);
            }
        }

        private static void FloodFillDirection(
            Vector2Int startingPosition,
            Vector2Int direction,
            IReadOnlyCollection<Vector2Int> boardPositions,
            IList<Vector2Int> validMoves)
        {
            Vector2Int nextPosition = startingPosition + direction;
            while (boardPositions.Contains(nextPosition))
            {
                validMoves.Add(nextPosition);
                nextPosition += direction;
            }
        }
    }
}