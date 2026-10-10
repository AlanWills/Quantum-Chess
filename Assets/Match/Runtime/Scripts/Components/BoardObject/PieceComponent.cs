using System;
using System.Collections.Generic;
using System.ComponentModel;
using Celeste.Components;
using QC.Match.Actions;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Components
{
    [DisplayName("Piece")]
    public class PieceComponent : BoardObjectComponent, 
        IPiece, 
        IActionProvider, 
        IObjectStatus
    {
        #region Save Data

        [Serializable]
        private class SaveData : ComponentData
        {
            public PieceColour currentPieceColour;
            public PieceType currentPieceType;
            public int currentValue;
            public PieceVerticalDirection verticalDirection;
            public bool hasMoved;
            public List<ActionPattern> movementPatterns = new();
            public List<ActionPattern> attackPatterns = new();
        }
        
        #endregion
        
        #region Properties and Fields
        
        [SerializeField] private PieceColour startingColour = PieceColour.None;
        [SerializeField] private PieceType startingPieceType = PieceTypeExtensions.Everything;
        [SerializeField] private int startingValue = 1;
        [SerializeField] private List<ActionPattern> startingMovementPatterns = new();
        [SerializeField] private List<ActionPattern> startingAttackPatterns = new();
        
        #endregion

        public override ComponentData CreateData()
        {
            return new SaveData
            {
                currentPieceColour = startingColour,
                currentPieceType = startingPieceType,
                currentValue = startingValue,
                verticalDirection = PieceVerticalDirection.Up,
                hasMoved = false,
                movementPatterns = new List<ActionPattern>(startingMovementPatterns),
                attackPatterns = new List<ActionPattern>(startingAttackPatterns)
            };
        }

        public PieceColour GetPieceColour(Instance instance)
        {
            return (instance.data as SaveData).currentPieceColour;
        }

        public void SetColour(Instance instance, PieceColour colour)
        {
            SaveData saveData = instance.data as SaveData;
            if (saveData.currentPieceColour != colour)
            {
                saveData.currentPieceColour = colour;
                instance.events.ComponentDataChanged.Invoke();
            }
        }

        public PieceType GetPieceType(Instance instance)
        {
            return (instance.data as SaveData).currentPieceType;
        }
        
        public void SetType(Instance instance, PieceType type)
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

        public void SetVerticalDirection(Instance instance, PieceVerticalDirection verticalDirection)
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
            IBoardActionMask opponentBoardActionMask,
            IList<Vector2Int> validMoves)
        {
            SaveData saveData = instance.data as SaveData;
            
            CalculateValidMoves(
                saveData.movementPatterns,
                (int)saveData.verticalDirection,
                saveData.hasMoved,
                currentBoardPosition,
                boardLookup,
                opponentBoardActionMask,
                validMoves);
        }

        private static void CalculateValidMoves(
            IReadOnlyList<ActionPattern> movementPatterns,
            int verticalDirection,
            bool hasMoved,
            Vector2Int currentBoardPosition, 
            IBoardLookup boardLookup,
            IBoardActionMask opponentBoardActionMask,
            IList<Vector2Int> validMoves)
        {
            foreach (ActionPattern actionPattern in movementPatterns)
            {
                CalculateValidMoves(
                    actionPattern, 
                    verticalDirection, 
                    hasMoved, 
                    currentBoardPosition, 
                    boardLookup, 
                    opponentBoardActionMask, 
                    validMoves);
            }
        }

        private static void CalculateValidMoves(
            ActionPattern actionPattern,
            int verticalDirection,
            bool hasMoved,
            Vector2Int currentBoardPosition,
            IBoardLookup boardLookup,
            IBoardActionMask opponentBoardActionMask,
            IList<Vector2Int> validMoves)
        {
            Vector2Int pattern = actionPattern.relativeToVerticalDirection ? actionPattern.pattern * verticalDirection : actionPattern.pattern;
            
            switch (actionPattern.range)
            {
                case RangeType.Infinite:
                    if (actionPattern.mustBeSafe)
                    {
                        TryFindSafeMovesInDirection(currentBoardPosition, pattern, boardLookup, opponentBoardActionMask, validMoves);
                    }
                    else
                    {
                        TryFindMovesInDirection(currentBoardPosition, pattern, boardLookup, validMoves);
                    }
                    break;
                
                case RangeType.Finite:
                    int distanceToCheck = hasMoved ? actionPattern.distanceIfMoved : actionPattern.distanceIfNotMoved;
                    for (int i = 1; i <= distanceToCheck; ++i)
                    {
                        if (actionPattern.mustBeSafe)
                        {
                            AddIfVacantAndSafeBoardPosition(
                                currentBoardPosition + pattern * i,
                                boardLookup,
                                opponentBoardActionMask,
                                validMoves);
                        }
                        else
                        {
                            AddIfVacantBoardPosition(
                                currentBoardPosition + pattern * i, 
                                boardLookup,
                                validMoves);    
                        }
                    }
                    break;
                
                default:
                    UnityEngine.Debug.LogAssertion($"Invalid {nameof(RangeType)} value {actionPattern.range} when trying to calculate valid moves.");
                    break;
            }
        }

        public void CalculateValidAttacks(
            Instance instance,
            Vector2Int currentBoardPosition, 
            IBoardLookup boardLookup,
            IBoardActionMask opponentBoardActionMask,
            IList<Vector2Int> validAttacks)
        {
            SaveData saveData = instance.data as SaveData;
            
            CalculateValidAttacks(
                saveData.attackPatterns,
                saveData.currentPieceColour,
                (int)saveData.verticalDirection,
                saveData.hasMoved,
                currentBoardPosition,
                boardLookup,
                opponentBoardActionMask,
                validAttacks);
        }

        private static void CalculateValidAttacks(
            IReadOnlyList<ActionPattern> attackPatterns,
            PieceColour currentColour,
            int verticalDirection,
            bool hasMoved,
            Vector2Int currentBoardPosition, 
            IBoardLookup boardLookup,
            IBoardActionMask opponentBoardActionMask,
            IList<Vector2Int> validAttacks)
        {
            foreach (ActionPattern actionPattern in attackPatterns)
            {
                CalculateValidAttacks(
                    actionPattern, 
                    currentColour, 
                    verticalDirection, 
                    hasMoved, 
                    currentBoardPosition, 
                    boardLookup, 
                    opponentBoardActionMask, 
                    validAttacks);
            }
        }

        private static void CalculateValidAttacks(
            ActionPattern actionPattern,
            PieceColour currentColour,
            int verticalDirection,
            bool hasMoved,
            Vector2Int currentBoardPosition,
            IBoardLookup boardLookup,
            IBoardActionMask opponentBoardActionMask,
            IList<Vector2Int> validAttacks)
        {
            Vector2Int pattern = actionPattern.relativeToVerticalDirection ? actionPattern.pattern * verticalDirection : actionPattern.pattern;
            
            switch (actionPattern.range)
            {
                case RangeType.Infinite:
                    if (actionPattern.mustBeSafe)
                    {
                        TryFindSafeAttackablePiecesInDirection(
                            currentBoardPosition, 
                            pattern, 
                            currentColour, 
                            boardLookup, 
                            opponentBoardActionMask, 
                            validAttacks);
                    }
                    else
                    {
                        TryFindAttackablePiecesInDirection(
                            currentBoardPosition, 
                            pattern, 
                            currentColour, 
                            boardLookup, 
                            validAttacks);
                    }
                    break;
                
                case RangeType.Finite:
                    int distanceToCheck = hasMoved ? actionPattern.distanceIfMoved : actionPattern.distanceIfNotMoved;
                    for (int i = 1; i <= distanceToCheck; ++i)
                    {
                        if (actionPattern.mustBeSafe)
                        {
                            TryFindSafeAttackablePiece(
                                currentBoardPosition + pattern * i,
                                currentColour,
                                boardLookup,
                                opponentBoardActionMask,
                                validAttacks);
                        }
                        else
                        {
                            TryFindAttackablePiece(
                                currentBoardPosition + pattern * i,
                                currentColour,
                                boardLookup,
                                validAttacks);
                        }
                    }
                    break;
                
                default:
                    UnityEngine.Debug.LogAssertion($"Invalid {nameof(RangeType)} value {actionPattern.range} when trying to calculate valid moves.");
                    break;
            }
        }

        public void CalculateValidThreats(
            Instance instance, 
            Vector2Int currentBoardPosition, 
            IBoardLookup boardLookup,
            IList<Vector2Int> validThreats)
        {
            SaveData saveData = instance.data as SaveData;
            
            CalculateValidThreats(
                saveData.attackPatterns,
                saveData.currentPieceColour,
                (int)saveData.verticalDirection,
                saveData.hasMoved,
                currentBoardPosition,
                boardLookup,
                validThreats);
        }

        private static void CalculateValidThreats(
            IReadOnlyList<ActionPattern> attackPatterns,
            PieceColour currentColour,
            int verticalDirection,
            bool hasMoved,
            Vector2Int currentBoardPosition, 
            IBoardLookup boardLookup,
            IList<Vector2Int> validThreats)
        {
            foreach (ActionPattern actionPattern in attackPatterns)
            {
                CalculateValidThreats(
                    actionPattern,
                    currentColour,
                    verticalDirection,
                    hasMoved,
                    currentBoardPosition,
                    boardLookup,
                    validThreats);
            }
        }

        private static void CalculateValidThreats(
            ActionPattern actionPattern,
            PieceColour currentColour,
            int verticalDirection,
            bool hasMoved,
            Vector2Int currentBoardPosition,
            IBoardLookup boardLookup,
            IList<Vector2Int> validThreats)
        {
            Vector2Int pattern = actionPattern.relativeToVerticalDirection ? actionPattern.pattern * verticalDirection : actionPattern.pattern;
            
            switch (actionPattern.range)
            {
                case RangeType.Infinite:
                    TryFindAttackableTilesInDirection(
                        currentBoardPosition, 
                        pattern, 
                        currentColour, 
                        boardLookup,
                        validThreats);
                    break;
                
                case RangeType.Finite:
                    int distanceToCheck = hasMoved ? actionPattern.distanceIfMoved : actionPattern.distanceIfNotMoved;
                    for (int i = 1; i <= distanceToCheck; ++i)
                    {
                        AddIfBoardPositionExists(
                            currentBoardPosition + pattern * i, 
                            boardLookup, 
                            validThreats);
                    }
                    break;
                
                default:
                    UnityEngine.Debug.LogAssertion($"Invalid {nameof(RangeType)} value {actionPattern.range} when trying to calculate valid moves.");
                    break;
            }
        }

        private static bool IsBoardPositionVacant(Vector2Int position, IBoardLookup boardLookup)
        {
            return boardLookup.DoesBoardTileExist(position) && boardLookup.GetBoardObjectAt(position) == null;
        }

        private static bool AddIfBoardPositionExists(
            Vector2Int position,
            IBoardLookup boardLookup,
            IList<Vector2Int> list)
        {
            if (!boardLookup.DoesBoardTileExist(position))
            {
                return false;
            }
            
            list.Add(position);
            return true;
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

        private static bool AddIfVacantAndSafeBoardPosition(
            Vector2Int position,
            IBoardLookup boardLookup,
            IBoardActionMask opponentBoardActionMask,
            IList<Vector2Int> list)
        {
            if (!IsBoardPositionVacant(position, boardLookup))
            {
                return false;
            }

            if (opponentBoardActionMask.IsTileAttackable(position))
            {
                return false;
            }
            
            list.Add(position);
            return true;
        }

        private static bool TryFindAttackablePiece(
            Vector2Int position,
            PieceColour currentColour,
            IBoardLookup boardLookup,
            IList<Vector2Int> list)
        {
            BoardObjectInstance objectInstance = boardLookup.GetBoardObjectAt(position);

            if (objectInstance == null || !objectInstance.TryFindInterface<IPiece>(out var chessPiece))
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

        private static bool TryFindSafeAttackablePiece(
            Vector2Int position,
            PieceColour currentColour,
            IBoardLookup boardLookup,
            IBoardActionMask opponentBoardActionMask,
            IList<Vector2Int> list)
        {
            if (opponentBoardActionMask.IsTileAttackable(position))
            {
                return false;
            }

            return TryFindAttackablePiece(position, currentColour, boardLookup, list);
        }

        private static void TryFindMovesInDirection(
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

        private static void TryFindSafeMovesInDirection(
            Vector2Int startingPosition,
            Vector2Int direction,
            IBoardLookup boardLookup,
            IBoardActionMask opponentBoardActionMask,
            IList<Vector2Int> validMoves)
        {
            Vector2Int nextPosition = startingPosition + direction;
            while (AddIfVacantAndSafeBoardPosition(nextPosition, boardLookup, opponentBoardActionMask, validMoves))
            {
                nextPosition += direction;
            }
        }

        private static void TryFindSafeAttackablePiecesInDirection(
            Vector2Int startingPosition,
            Vector2Int direction,
            PieceColour currentColour,
            IBoardLookup boardLookup,
            IBoardActionMask opponentBoardActionMask,
            IList<Vector2Int> validAttacks)
        {
            Vector2Int nextPosition = startingPosition + direction;
            
            // Stop at the first piece we find in this direction
            // Even friendlies will block subsequent attacks
            while (boardLookup.DoesBoardTileExist(nextPosition) &&
                   !TryFindSafeAttackablePiece(nextPosition, currentColour, boardLookup, opponentBoardActionMask, validAttacks))
            {
                nextPosition += direction;
            }
        }

        private static void TryFindAttackablePiecesInDirection(
            Vector2Int startingPosition,
            Vector2Int direction,
            PieceColour currentColour,
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

        private static void TryFindSafeAttackableTilesInDirection(
            Vector2Int startingPosition,
            Vector2Int direction,
            PieceColour currentColour,
            IBoardLookup boardLookup,
            IBoardActionMask opponentBoardActionMask,
            IList<Vector2Int> validAttackableTiles)
        {
            Vector2Int nextPosition = startingPosition + direction;
            
            // Stop at the first piece we find in this direction
            // Even friendlies will block subsequent attacks
            while (boardLookup.DoesBoardTileExist(nextPosition))
            {
                validAttackableTiles.Add(nextPosition);

                if (TryFindSafeAttackablePiece(nextPosition, currentColour, boardLookup, opponentBoardActionMask, validAttackableTiles))
                {
                    break;
                }
                
                nextPosition += direction;
            }
        }

        private static void TryFindAttackableTilesInDirection(
            Vector2Int startingPosition,
            Vector2Int direction,
            PieceColour currentColour,
            IBoardLookup boardLookup,
            IList<Vector2Int> validAttackableTiles)
        {
            Vector2Int nextPosition = startingPosition + direction;
            
            // Stop at the first piece we find in this direction
            // Even friendlies will block subsequent attacks
            while (boardLookup.DoesBoardTileExist(nextPosition))
            {
                validAttackableTiles.Add(nextPosition);

                if (TryFindAttackablePiece(nextPosition, currentColour, boardLookup, validAttackableTiles))
                {
                    break;
                }
                
                nextPosition += direction;
            }
        }
    }
}