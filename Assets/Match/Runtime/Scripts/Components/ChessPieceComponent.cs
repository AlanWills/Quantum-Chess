using System;
using System.Collections.Generic;
using System.ComponentModel;
using Celeste.Components;
using Celeste.DataStructures;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Components
{
    [DisplayName("Chess Piece Type")]
    public class ChessPieceComponent : ChessBoardObjectComponent, 
        IChessPiece, 
        IObjectActionProvider, 
        IObjectStatus
    {
        #region Save Data

        [Serializable]
        public class SaveData : ComponentData
        {
            public ChessPieceColour currentPieceColour;
            public ChessPieceType currentPieceType;
            public bool hasMoved;
            public ChessPieceVerticalDirection verticalDirection;
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
                currentPieceType = startingPieceType,
                hasMoved = false,
                verticalDirection = ChessPieceVerticalDirection.Up
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

        public void SetVerticalDirection(Instance instance, ChessPieceVerticalDirection verticalDirection)
        {
            SaveData saveData = instance.data as SaveData;
            if (saveData.verticalDirection != verticalDirection)
            {
                saveData.verticalDirection = verticalDirection;
                instance.events.ComponentDataChanged.Invoke();
            }
        }

        public void SetHasMoved(Instance instance, bool hasMoved)
        {
            SaveData saveData = instance.data as SaveData;
            if (saveData.hasMoved != hasMoved)
            {
                saveData.hasMoved = hasMoved;
                instance.events.ComponentDataChanged.Invoke();
            }
        }

        public void CalculateValidMoves(
            Instance instance, 
            Vector2Int currentBoardPosition, 
            IChessBoardLookup chessBoardLookup, 
            IList<Vector2Int> validMoves)
        {
            SaveData saveData = instance.data as SaveData;
            for (int i = 1; i < (int)ChessPieceTypeExtensions.Everything; i <<= 1)
            {
                ChessPieceType currentType = (ChessPieceType)i;
                if (!saveData.currentPieceType.HasFlag(currentType))
                {
                    continue;
                }
                
                CalculateValidMoves(
                    currentType,
                    (int)saveData.verticalDirection,
                    saveData.hasMoved,
                    currentBoardPosition,
                    chessBoardLookup,
                    validMoves);
            }
        }

        public void CalculateValidAttacks(
            Instance instance, 
            Vector2Int currentBoardPosition, 
            IChessBoardLookup chessBoardLookup,
            IList<Vector2Int> validAttacks)
        {
            SaveData saveData = instance.data as SaveData;
            for (int i = 1; i < (int)ChessPieceTypeExtensions.Everything; i <<= 1)
            {
                ChessPieceType currentType = (ChessPieceType)i;
                if (!saveData.currentPieceType.HasFlag(currentType))
                {
                    continue;
                }
                
                CalculateValidAttacks(
                    currentType,
                    saveData.currentPieceColour,
                    (int)saveData.verticalDirection,
                    currentBoardPosition,
                    chessBoardLookup,
                    validAttacks);
            }
        }

        private static void CalculateValidMoves(
            ChessPieceType type,
            int verticalDirection,
            bool hasMoved,
            Vector2Int currentBoardPosition, 
            IChessBoardLookup chessBoardLookup, 
            IList<Vector2Int> validMoves)
        {
            switch (type)
            {
                case ChessPieceType.None:
                    return;

                case ChessPieceType.Pawn:
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(0, 1 * verticalDirection),
                        chessBoardLookup, validMoves);

                    if (!hasMoved)
                    {
                        AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(0, 2 * verticalDirection),
                            chessBoardLookup, validMoves);
                    }

                    return;

                case ChessPieceType.Knight:
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(-2, -1), chessBoardLookup,
                        validMoves);
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(-2, 1), chessBoardLookup,
                        validMoves);
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(2, -1), chessBoardLookup,
                        validMoves);
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(2, 1), chessBoardLookup,
                        validMoves);
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(-1, -2), chessBoardLookup,
                        validMoves);
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(-1, 2), chessBoardLookup,
                        validMoves);
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(1, -2), chessBoardLookup,
                        validMoves);
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(1, 2), chessBoardLookup,
                        validMoves);

                    return;

                case ChessPieceType.Bishop:
                    FloodFillDirection(currentBoardPosition, new Vector2Int(-1, -1), chessBoardLookup, validMoves);
                    FloodFillDirection(currentBoardPosition, new Vector2Int(-1, 1), chessBoardLookup, validMoves);
                    FloodFillDirection(currentBoardPosition, new Vector2Int(1, -1), chessBoardLookup, validMoves);
                    FloodFillDirection(currentBoardPosition, new Vector2Int(1, 1), chessBoardLookup, validMoves);

                    return;

                case ChessPieceType.Rook:
                    FloodFillDirection(currentBoardPosition, new Vector2Int(-1, 0), chessBoardLookup, validMoves);
                    FloodFillDirection(currentBoardPosition, new Vector2Int(1, 0), chessBoardLookup, validMoves);
                    FloodFillDirection(currentBoardPosition, new Vector2Int(0, -1), chessBoardLookup, validMoves);
                    FloodFillDirection(currentBoardPosition, new Vector2Int(0, 1), chessBoardLookup, validMoves);

                    return;

                case ChessPieceType.Queen:
                    FloodFillDirection(currentBoardPosition, new Vector2Int(-1, -1), chessBoardLookup, validMoves);
                    FloodFillDirection(currentBoardPosition, new Vector2Int(-1, 1), chessBoardLookup, validMoves);
                    FloodFillDirection(currentBoardPosition, new Vector2Int(1, -1), chessBoardLookup, validMoves);
                    FloodFillDirection(currentBoardPosition, new Vector2Int(1, 1), chessBoardLookup, validMoves);
                    FloodFillDirection(currentBoardPosition, new Vector2Int(-1, 0), chessBoardLookup, validMoves);
                    FloodFillDirection(currentBoardPosition, new Vector2Int(1, 0), chessBoardLookup, validMoves);
                    FloodFillDirection(currentBoardPosition, new Vector2Int(0, -1), chessBoardLookup, validMoves);
                    FloodFillDirection(currentBoardPosition, new Vector2Int(0, 1), chessBoardLookup, validMoves);

                    return;

                case ChessPieceType.King:
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(-1, -1), chessBoardLookup,
                        validMoves);
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(-1, 0), chessBoardLookup,
                        validMoves);
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(-1, 1), chessBoardLookup,
                        validMoves);
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(1, -1), chessBoardLookup,
                        validMoves);
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(1, 0), chessBoardLookup,
                        validMoves);
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(1, 1), chessBoardLookup,
                        validMoves);
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(0, -1), chessBoardLookup,
                        validMoves);
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(0, 1), chessBoardLookup,
                        validMoves);
                    return;
            }
        }

        private static void CalculateValidAttacks(
            ChessPieceType type,
            ChessPieceColour currentColour,
            int verticalDirection,
            Vector2Int currentBoardPosition, 
            IChessBoardLookup chessBoardLookup,
            IList<Vector2Int> validAttacks)
        {
            switch (type)
            {
                case ChessPieceType.None:
                    return;
               
                case ChessPieceType.Pawn:
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(-1, 1 * verticalDirection), currentColour, chessBoardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(1, 1 * verticalDirection), currentColour, chessBoardLookup, validAttacks);
                    
                    return;
               
                case ChessPieceType.Knight:
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(-2, -1), currentColour, chessBoardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(-2, 1), currentColour, chessBoardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(2, -1), currentColour, chessBoardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(2, 1), currentColour, chessBoardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(-1, -2), currentColour, chessBoardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(-1, 2), currentColour, chessBoardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(1, -2), currentColour, chessBoardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(1, 2), currentColour, chessBoardLookup, validAttacks);
                    
                    return;
               
                case ChessPieceType.Bishop:
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(-1, -1), currentColour, chessBoardLookup, validAttacks);
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(-1, 1), currentColour, chessBoardLookup, validAttacks);
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(1, -1), currentColour, chessBoardLookup, validAttacks);
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(1, 1), currentColour, chessBoardLookup, validAttacks);
                    
                    return;
               
                case ChessPieceType.Rook:
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(-1, 0), currentColour, chessBoardLookup, validAttacks);
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(1, 0), currentColour, chessBoardLookup, validAttacks);
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(0, -1), currentColour, chessBoardLookup, validAttacks);
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(0, 1), currentColour, chessBoardLookup, validAttacks);
                    
                    return;
               
                case ChessPieceType.Queen:
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(-1, -1), currentColour, chessBoardLookup, validAttacks);
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(-1, 1), currentColour, chessBoardLookup, validAttacks);
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(1, -1), currentColour, chessBoardLookup, validAttacks);
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(1, 1), currentColour, chessBoardLookup, validAttacks);
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(-1, 0), currentColour, chessBoardLookup, validAttacks);
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(1, 0), currentColour, chessBoardLookup, validAttacks);
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(0, -1), currentColour, chessBoardLookup, validAttacks);
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(0, 1), currentColour, chessBoardLookup, validAttacks);
                    
                    return;
               
                case ChessPieceType.King:
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(-1, -1), currentColour, chessBoardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(-1, 0), currentColour, chessBoardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(-1, 1), currentColour, chessBoardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(1, -1), currentColour, chessBoardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(1, 0), currentColour, chessBoardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(1, 1), currentColour, chessBoardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(0, -1), currentColour, chessBoardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(0, 1), currentColour, chessBoardLookup, validAttacks);
                    
                    return;
            }
        }

        private static bool AddIfVacantBoardPosition(
            Vector2Int position,
            IChessBoardLookup chessBoardLookup,
            IList<Vector2Int> list)
        {
            if (!chessBoardLookup.DoesChessBoardTileExist(position) ||
                chessBoardLookup.GetChessBoardObjectAt(position) != null)
            {
                return false;
            }
            
            list.Add(position);
            return true;
        }

        private static bool TryFindAttackablePiece(
            Vector2Int position,
            ChessPieceColour currentColour,
            IChessBoardLookup chessBoardLookup,
            IList<Vector2Int> list)
        {
            ChessBoardObjectInstance objectInstance = chessBoardLookup.GetChessBoardObjectAt(position);

            if (objectInstance == null || !objectInstance.TryFindComponent<IChessPiece>(out var chessPiece))
            {
                return false;
            }

            if (chessPiece.iFace.GetPieceColour(chessPiece.instance) != currentColour)
            {
                // Attacks are only valid on non-matching colours, but all pieces block attacks
                list.Add(position);
            }

            return true;
        }

        private static void FloodFillDirection(
            Vector2Int startingPosition,
            Vector2Int direction,
            IChessBoardLookup chessBoardLookup,
            IList<Vector2Int> validMoves)
        {
            Vector2Int nextPosition = startingPosition + direction;
            while (AddIfVacantBoardPosition(nextPosition, chessBoardLookup, validMoves))
            {
                nextPosition += direction;
            }
        }

        private static void TryFindAttackablePiecesInDirection(
            Vector2Int startingPosition,
            Vector2Int direction,
            ChessPieceColour currentColour,
            IChessBoardLookup chessBoardLookup,
            IList<Vector2Int> validAttacks)
        {
            Vector2Int nextPosition = startingPosition + direction;
            
            // Stop at the first piece we find in this direction
            // Even friendlies will block subsequent attacks
            while (chessBoardLookup.DoesChessBoardTileExist(nextPosition) &&
                   !TryFindAttackablePiece(nextPosition, currentColour, chessBoardLookup, validAttacks))
            {
                nextPosition += direction;
            }
        }
    }
}