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
            UnityEngine.Debug.Assert(!actionPattern.relativeToVerticalDirection || verticalDirection != 0, $"Attempting to calculate relative movement with a 0 vertical direction!");
            Vector2Int pattern = actionPattern.relativeToVerticalDirection ? actionPattern.pattern * verticalDirection : actionPattern.pattern;
            int distanceToCheck = hasMoved ? actionPattern.distanceIfMoved : actionPattern.distanceIfNotMoved;
            
            TryFindMovesInDirection(
                currentBoardPosition, 
                pattern,
                actionPattern.moveType, 
                actionPattern.mustBeSafe,
                actionPattern.range == RangeType.Finite ? distanceToCheck : int.MaxValue,
                boardLookup, 
                opponentBoardActionMask, 
                validMoves);
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
            UnityEngine.Debug.Assert(!actionPattern.relativeToVerticalDirection || verticalDirection != 0, $"Attempting to calculate relative attacks with a 0 vertical direction!");
            Vector2Int pattern = actionPattern.relativeToVerticalDirection ? actionPattern.pattern * verticalDirection : actionPattern.pattern;
            int distanceToCheck = hasMoved ? actionPattern.distanceIfMoved : actionPattern.distanceIfNotMoved;
            
            TryFindAttackablePiecesInDirection(
                currentBoardPosition,
                pattern, 
                currentColour,
                actionPattern.moveType,
                actionPattern.mustBeSafe,
                actionPattern.range == RangeType.Finite ? distanceToCheck : int.MaxValue,
                boardLookup,
                opponentBoardActionMask,
                validAttacks);
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
            
            UnityEngine.Debug.Assert(!actionPattern.relativeToVerticalDirection || verticalDirection != 0, $"Attempting to calculate relative threats with a 0 vertical direction!");
            Vector2Int pattern = actionPattern.relativeToVerticalDirection ? actionPattern.pattern * verticalDirection : actionPattern.pattern;
            int distanceToCheck = hasMoved ? actionPattern.distanceIfMoved : actionPattern.distanceIfNotMoved;
            
            TryFindAttackablePiecesInDirection(
                currentBoardPosition,
                pattern, 
                currentColour,
                actionPattern.moveType,
                false,
                actionPattern.range == RangeType.Finite ? distanceToCheck : int.MaxValue,
                boardLookup,
                new EmptyBoardActionMask(),
                validThreats);
        }

        private static bool IsValidAndVacantBoardPosition(
            Vector2Int position,
            bool mustBeSafe,
            IBoardLookup boardLookup,
            IBoardActionMask opponentBoardActionMask)
        {
            if (!boardLookup.IsPositionInBoard(position))
            {
                return false;
            }

            if (!boardLookup.DoesBoardTileExist(position))
            {
                return false;
            }
            
            if (boardLookup.GetBoardObjectAt(position) != null)
            {
                return false;
            }

            if (mustBeSafe && opponentBoardActionMask.IsTileAttackable(position))
            {
                return false;
            }
            
            return true;
        }

        private static bool IsValidAndAttackableBoardPosition(
            Vector2Int position,
            PieceColour currentColour,
            bool mustBeSafe,
            IBoardLookup boardLookup,
            IBoardActionMask opponentBoardActionMask)
        {
            if (!boardLookup.IsPositionInBoard(position))
            {
                return false;
            }

            if (!boardLookup.DoesBoardTileExist(position))
            {
                return false;
            }
            
            if (mustBeSafe && opponentBoardActionMask.IsTileAttackable(position))
            {
                return false;
            }
            
            BoardObjectInstance objectInstance = boardLookup.GetBoardObjectAt(position);
            if (objectInstance == null || !objectInstance.TryFindInterface<IPiece>(out var chessPiece))
            {
                return false;
            }

            if (chessPiece.GetPieceColour() == currentColour)
            {
                return false;
            }
            
            return true;
        }

        private static void TryFindMovesInDirection(
            Vector2Int startingPosition,
            Vector2Int direction,
            MoveType moveType,
            bool mustBeSafe,
            int maxDistanceToSearch,
            IBoardLookup boardLookup,
            IBoardActionMask opponentActionMask,
            IList<Vector2Int> validMoves)
        {
            for (int i = 1; i <= maxDistanceToSearch; ++i)
            {
                Vector2Int nextPosition = startingPosition + i * direction;

                // If this position fails one of the criteria we've mandated and we're doing continuous movement we should finish searching
                if (!IsValidAndVacantBoardPosition(nextPosition, mustBeSafe, boardLookup, opponentActionMask) &&
                    moveType == MoveType.Continuous)
                {
                    break;
                }
                
                validMoves.Add(nextPosition);
            }
        }

        private static void TryFindAttackablePiecesInDirection(
            Vector2Int startingPosition,
            Vector2Int direction,
            PieceColour currentColour,
            MoveType moveType,
            bool mustBeSafe,
            int maxDistanceToSearch,
            IBoardLookup boardLookup,
            IBoardActionMask opponentActionMask,
            IList<Vector2Int> validAttacks)
        {
            for (int i = 1; i <= maxDistanceToSearch; ++i)
            {
                Vector2Int nextPosition = startingPosition + i * direction;

                // If this position fails one of the criteria we've mandated and we're doing continuous movement we should finish searching
                if (!IsValidAndAttackableBoardPosition(nextPosition, currentColour, mustBeSafe, boardLookup, opponentActionMask) &&
                    moveType == MoveType.Continuous)
                {
                    break;
                }
                
                validAttacks.Add(nextPosition);
            }
        }
    }
}