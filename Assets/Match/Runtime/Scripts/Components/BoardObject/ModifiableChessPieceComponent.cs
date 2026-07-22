using System;
using System.Collections.Generic;
using System.ComponentModel;
using Celeste.Components;
using Celeste.DataStructures;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Components
{
    [DisplayName("Modifiable Chess Piece")]
    public class ModifiableChessPieceComponent : BoardObjectComponent, 
        IModifiableChessPiece, 
        IActionProvider, 
        IObjectStatus
    {
        #region Save Data

        [Serializable]
        public class SaveData : ComponentData
        {
            public ChessPieceColour currentPieceColour;
            public ChessPieceType currentPieceType;
            public int currentValue;
            public ChessPieceVerticalDirection verticalDirection;
            public bool hasMoved;
        }
        
        #endregion
        
        #region Properties and Fields
        
        [SerializeField] private ChessPieceColour startingColour = ChessPieceColour.Unknown;
        [SerializeField] private ChessPieceType startingPieceType = ChessPieceTypeExtensions.Everything;
        [SerializeField] private int startingValue = 1;
        
        #endregion

        public override ComponentData CreateData()
        {
            return new SaveData
            {
                currentPieceColour = startingColour,
                currentPieceType = startingPieceType,
                currentValue = startingValue,
                verticalDirection = ChessPieceVerticalDirection.Up,
                hasMoved = false,
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

        public int GetValue(Instance instance)
        {
            return (instance.data as SaveData).currentValue;
        }

        public void SetValue(Instance instance, int value)
        {
            SaveData saveData = instance.data as SaveData;
            if (saveData.currentValue != value)
            {
                saveData.currentValue = value;
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
            IBoardLookup boardLookup,
            IPlayerActionMaskLookup playerActionMaskLookup,
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
                    boardLookup,
                    validMoves);
            }
        }

        public void CalculateValidAttacks(
            Instance instance, 
            Vector2Int currentBoardPosition, 
            IBoardLookup boardLookup,
            IPlayerActionMaskLookup playerActionMaskLookup,
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
                    boardLookup,
                    validAttacks);
            }
        }

        private static void CalculateValidMoves(
            ChessPieceType type,
            int verticalDirection,
            bool hasMoved,
            Vector2Int currentBoardPosition, 
            IBoardLookup boardLookup,
            IList<Vector2Int> validMoves)
        {
            switch (type)
            {
                case ChessPieceType.None:
                    return;

                case ChessPieceType.Pawn:
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(0, 1 * verticalDirection),
                        boardLookup, validMoves);

                    // The space in front must be free for the pawn to move two
                    if (!hasMoved && IsBoardPositionVacant(currentBoardPosition + new Vector2Int(0, 1 * verticalDirection), boardLookup))
                    {
                        AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(0, 2 * verticalDirection),
                            boardLookup, validMoves);
                    }

                    return;

                case ChessPieceType.Knight:
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(-2, -1), boardLookup,
                        validMoves);
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(-2, 1), boardLookup,
                        validMoves);
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(2, -1), boardLookup,
                        validMoves);
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(2, 1), boardLookup,
                        validMoves);
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(-1, -2), boardLookup,
                        validMoves);
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(-1, 2), boardLookup,
                        validMoves);
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(1, -2), boardLookup,
                        validMoves);
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(1, 2), boardLookup,
                        validMoves);

                    return;

                case ChessPieceType.Bishop:
                    FloodFillDirection(currentBoardPosition, new Vector2Int(-1, -1), boardLookup, validMoves);
                    FloodFillDirection(currentBoardPosition, new Vector2Int(-1, 1), boardLookup, validMoves);
                    FloodFillDirection(currentBoardPosition, new Vector2Int(1, -1), boardLookup, validMoves);
                    FloodFillDirection(currentBoardPosition, new Vector2Int(1, 1), boardLookup, validMoves);

                    return;

                case ChessPieceType.Rook:
                    FloodFillDirection(currentBoardPosition, new Vector2Int(-1, 0), boardLookup, validMoves);
                    FloodFillDirection(currentBoardPosition, new Vector2Int(1, 0), boardLookup, validMoves);
                    FloodFillDirection(currentBoardPosition, new Vector2Int(0, -1), boardLookup, validMoves);
                    FloodFillDirection(currentBoardPosition, new Vector2Int(0, 1), boardLookup, validMoves);

                    return;

                case ChessPieceType.Queen:
                    FloodFillDirection(currentBoardPosition, new Vector2Int(-1, -1), boardLookup, validMoves);
                    FloodFillDirection(currentBoardPosition, new Vector2Int(-1, 1), boardLookup, validMoves);
                    FloodFillDirection(currentBoardPosition, new Vector2Int(1, -1), boardLookup, validMoves);
                    FloodFillDirection(currentBoardPosition, new Vector2Int(1, 1), boardLookup, validMoves);
                    FloodFillDirection(currentBoardPosition, new Vector2Int(-1, 0), boardLookup, validMoves);
                    FloodFillDirection(currentBoardPosition, new Vector2Int(1, 0), boardLookup, validMoves);
                    FloodFillDirection(currentBoardPosition, new Vector2Int(0, -1), boardLookup, validMoves);
                    FloodFillDirection(currentBoardPosition, new Vector2Int(0, 1), boardLookup, validMoves);

                    return;

                case ChessPieceType.King:
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(-1, -1), boardLookup,
                        validMoves);
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(-1, 0), boardLookup,
                        validMoves);
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(-1, 1), boardLookup,
                        validMoves);
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(1, -1), boardLookup,
                        validMoves);
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(1, 0), boardLookup,
                        validMoves);
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(1, 1), boardLookup,
                        validMoves);
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(0, -1), boardLookup,
                        validMoves);
                    AddIfVacantBoardPosition(currentBoardPosition + new Vector2Int(0, 1), boardLookup,
                        validMoves);
                    return;
            }
        }

        private static void CalculateValidAttacks(
            ChessPieceType type,
            ChessPieceColour currentColour,
            int verticalDirection,
            Vector2Int currentBoardPosition, 
            IBoardLookup boardLookup,
            IList<Vector2Int> validAttacks)
        {
            switch (type)
            {
                case ChessPieceType.None:
                    return;
               
                case ChessPieceType.Pawn:
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(-1, 1 * verticalDirection), currentColour, boardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(1, 1 * verticalDirection), currentColour, boardLookup, validAttacks);
                    
                    return;
               
                case ChessPieceType.Knight:
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(-2, -1), currentColour, boardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(-2, 1), currentColour, boardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(2, -1), currentColour, boardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(2, 1), currentColour, boardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(-1, -2), currentColour, boardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(-1, 2), currentColour, boardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(1, -2), currentColour, boardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(1, 2), currentColour, boardLookup, validAttacks);
                    
                    return;
               
                case ChessPieceType.Bishop:
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(-1, -1), currentColour, boardLookup, validAttacks);
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(-1, 1), currentColour, boardLookup, validAttacks);
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(1, -1), currentColour, boardLookup, validAttacks);
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(1, 1), currentColour, boardLookup, validAttacks);
                    
                    return;
               
                case ChessPieceType.Rook:
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(-1, 0), currentColour, boardLookup, validAttacks);
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(1, 0), currentColour, boardLookup, validAttacks);
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(0, -1), currentColour, boardLookup, validAttacks);
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(0, 1), currentColour, boardLookup, validAttacks);
                    
                    return;
               
                case ChessPieceType.Queen:
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(-1, -1), currentColour, boardLookup, validAttacks);
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(-1, 1), currentColour, boardLookup, validAttacks);
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(1, -1), currentColour, boardLookup, validAttacks);
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(1, 1), currentColour, boardLookup, validAttacks);
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(-1, 0), currentColour, boardLookup, validAttacks);
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(1, 0), currentColour, boardLookup, validAttacks);
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(0, -1), currentColour, boardLookup, validAttacks);
                    TryFindAttackablePiecesInDirection(currentBoardPosition, new Vector2Int(0, 1), currentColour, boardLookup, validAttacks);
                    
                    return;
               
                case ChessPieceType.King:
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(-1, -1), currentColour, boardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(-1, 0), currentColour, boardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(-1, 1), currentColour, boardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(1, -1), currentColour, boardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(1, 0), currentColour, boardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(1, 1), currentColour, boardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(0, -1), currentColour, boardLookup, validAttacks);
                    TryFindAttackablePiece(currentBoardPosition + new Vector2Int(0, 1), currentColour, boardLookup, validAttacks);
                    
                    return;
            }
        }

        private static bool IsBoardPositionVacant(Vector2Int position, IBoardLookup boardLookup)
        {
            return boardLookup.DoesBoardTileExist(position) && boardLookup.GetBoardObjectAt(position) == null;
        }

        private static bool AddIfVacantBoardPosition(
            Vector2Int position,
            IBoardLookup boardLookup,
            IList<Vector2Int> list)
        {
            if (!IsBoardPositionVacant(position, boardLookup))
            {
                return false;
            }
            
            list.Add(position);
            return true;
        }

        private static bool TryFindAttackablePiece(
            Vector2Int position,
            ChessPieceColour currentColour,
            IBoardLookup boardLookup,
            IList<Vector2Int> list)
        {
            BoardObjectInstance objectInstance = boardLookup.GetBoardObjectAt(position);

            if (objectInstance == null || !objectInstance.TryFindComponent<IModifiableChessPiece>(out var chessPiece))
            {
                return false;
            }

            if (chessPiece.GetPieceColour() != currentColour)
            {
                // Attacks are only valid on non-matching colours, but all pieces block attacks
                list.Add(position);
            }

            return true;
        }

        private static void FloodFillDirection(
            Vector2Int startingPosition,
            Vector2Int direction,
            IBoardLookup boardLookup,
            IList<Vector2Int> validMoves)
        {
            Vector2Int nextPosition = startingPosition + direction;
            while (AddIfVacantBoardPosition(nextPosition, boardLookup, validMoves))
            {
                nextPosition += direction;
            }
        }

        private static void TryFindAttackablePiecesInDirection(
            Vector2Int startingPosition,
            Vector2Int direction,
            ChessPieceColour currentColour,
            IBoardLookup boardLookup,
            IList<Vector2Int> validAttacks)
        {
            Vector2Int nextPosition = startingPosition + direction;
            
            // Stop at the first piece we find in this direction
            // Even friendlies will block subsequent attacks
            while (boardLookup.DoesBoardTileExist(nextPosition) &&
                   !TryFindAttackablePiece(nextPosition, currentColour, boardLookup, validAttacks))
            {
                nextPosition += direction;
            }
        }
    }
}