using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
            int currentBoardPositionX,
            int currentBoardPositionY,
            IBoardLookup boardLookup,
            IBoardActionMask opponentBoardActionMask,
            IList<Vector2Int> validMoves)
        {
            SaveData saveData = instance.data as SaveData;
            
            CalculateValidMoves(
                saveData.movementPatterns,
                (int)saveData.verticalDirection,
                saveData.hasMoved,
                currentBoardPositionX,
                currentBoardPositionY,
                boardLookup,
                opponentBoardActionMask,
                validMoves);
        }

        private static void CalculateValidMoves(
            IReadOnlyList<ActionPattern> movementPatterns,
            int verticalDirection,
            bool hasMoved,
            int currentBoardPositionX,
            int currentBoardPositionY,
            IBoardLookup boardLookup,
            IBoardActionMask opponentBoardActionMask,
            IList<Vector2Int> validMoves)
        {
            for (int i = 0, n = movementPatterns.Count; i < n; ++i)
            {
                CalculateValidMoves(
                    movementPatterns[i], 
                    verticalDirection, 
                    hasMoved, 
                    currentBoardPositionX,
                    currentBoardPositionY,
                    boardLookup, 
                    opponentBoardActionMask, 
                    validMoves);
            }
        }

        private static void CalculateValidMoves(
            ActionPattern actionPattern,
            int verticalDirection,
            bool hasMoved,
            int currentBoardPositionX,
            int currentBoardPositionY,
            IBoardLookup boardLookup,
            IBoardActionMask opponentBoardActionMask,
            IList<Vector2Int> validMoves)
        {
#if ULTRA_DEBUG_CHECKS
            UnityEngine.Debug.Assert(!actionPattern.relativeToVerticalDirection || verticalDirection != 0, $"Attempting to calculate relative movement with a 0 vertical direction!");
#endif
            Vector2Int pattern = actionPattern.relativeToVerticalDirection ? actionPattern.pattern * verticalDirection : actionPattern.pattern;
            int distanceToCheck = hasMoved ? actionPattern.distanceIfMoved : actionPattern.distanceIfNotMoved;
            
            TryFindMovesInDirection(
                currentBoardPositionX,
                currentBoardPositionY,
                pattern.x,
                pattern.y,
                actionPattern.moveType, 
                actionPattern.mustBeSafe,
                actionPattern.range == RangeType.Finite ? distanceToCheck : int.MaxValue,
                boardLookup, 
                opponentBoardActionMask, 
                validMoves);
        }

        public void CalculateValidAttacks(
            Instance instance,
            int currentBoardPositionX,
            int currentBoardPositionY,
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
                currentBoardPositionX,
                currentBoardPositionY,
                boardLookup,
                opponentBoardActionMask,
                validAttacks);
        }

        private static void CalculateValidAttacks(
            IReadOnlyList<ActionPattern> attackPatterns,
            PieceColour currentColour,
            int verticalDirection,
            bool hasMoved,
            int currentBoardPositionX,
            int currentBoardPositionY,
            IBoardLookup boardLookup,
            IBoardActionMask opponentBoardActionMask,
            IList<Vector2Int> validAttacks)
        {
            for (int i = 0, n = attackPatterns.Count; i < n; ++i)
            {
                CalculateValidAttacks(
                    attackPatterns[i], 
                    currentColour, 
                    verticalDirection, 
                    hasMoved, 
                    currentBoardPositionX,
                    currentBoardPositionY,
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
            int currentBoardPositionX,
            int currentBoardPositionY,
            IBoardLookup boardLookup,
            IBoardActionMask opponentBoardActionMask,
            IList<Vector2Int> validAttacks)
        {
#if ULTRA_DEBUG_CHECKS
            UnityEngine.Debug.Assert(!actionPattern.relativeToVerticalDirection || verticalDirection != 0, $"Attempting to calculate relative attacks with a 0 vertical direction!");
#endif
            Vector2Int pattern = actionPattern.relativeToVerticalDirection ? actionPattern.pattern * verticalDirection : actionPattern.pattern;
            int distanceToCheck = hasMoved ? actionPattern.distanceIfMoved : actionPattern.distanceIfNotMoved;
            
            TryFindAttackablePiecesInDirection(
                currentBoardPositionX,
                currentBoardPositionY,
                pattern.x,
                pattern.y,
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
            int currentBoardPositionX,
            int currentBoardPositionY,
            IBoardLookup boardLookup,
            IList<Vector2Int> validThreats)
        {
            SaveData saveData = instance.data as SaveData;
            
            CalculateValidThreats(
                saveData.attackPatterns,
                (int)saveData.verticalDirection,
                saveData.hasMoved,
                currentBoardPositionX,
                currentBoardPositionY,
                boardLookup,
                validThreats);
        }

        private static void CalculateValidThreats(
            IReadOnlyList<ActionPattern> attackPatterns,
            int verticalDirection,
            bool hasMoved,
            int currentBoardPositionX,
            int currentBoardPositionY,
            IBoardLookup boardLookup,
            IList<Vector2Int> validThreats)
        {
            for (int i = 0, n = attackPatterns.Count; i < n; ++i)
            {
                CalculateValidThreats(
                    attackPatterns[i],
                    verticalDirection,
                    hasMoved,
                    currentBoardPositionX,
                    currentBoardPositionY,
                    boardLookup,
                    validThreats);
            }
        }

        private static void CalculateValidThreats(
            ActionPattern actionPattern,
            int verticalDirection,
            bool hasMoved,
            int currentBoardPositionX,
            int currentBoardPositionY,
            IBoardLookup boardLookup,
            IList<Vector2Int> validThreats)
        {
#if ULTRA_DEBUG_CHECKS
            UnityEngine.Debug.Assert(!actionPattern.relativeToVerticalDirection || verticalDirection != 0, "Attempting to calculate relative threats with a 0 vertical direction!");
#endif
            Vector2Int pattern = actionPattern.relativeToVerticalDirection ? actionPattern.pattern * verticalDirection : actionPattern.pattern;
            int distanceToCheck = hasMoved ? actionPattern.distanceIfMoved : actionPattern.distanceIfNotMoved;
            
            TryFindThreatenedPositionsInDirection(
                currentBoardPositionX,
                currentBoardPositionY,
                pattern.x,
                pattern.y,
                actionPattern.moveType,
                actionPattern.range == RangeType.Finite ? distanceToCheck : int.MaxValue,
                boardLookup,
                validThreats);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsBoardPositionInBounds(
            int x, 
            int y,
            IBoardLookup boardLookup)
        {
            return boardLookup.IsPositionInBoard(x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsValidBoardPosition(
            int x,
            int y,
            IBoardLookup boardLookup)
        {
            return boardLookup.DoesBoardTileExist(x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsVacantBoardPosition(
            int x,
            int y,
            IBoardLookup boardLookup)
        {
            return boardLookup.GetBoardObjectAt(x, y) == null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSafeBoardPosition(
            int x,
            int y,
            IBoardActionMask opponentBoardActionMask)
        {
            return !opponentBoardActionMask.IsTileAttackable(x, y);
        }

        private static bool IsAttackable(
            BoardObjectInstance objectInstance,
            PieceColour currentColour)
        {
            if (!objectInstance.Piece.IsValid)
            {
                return false;
            }

            if (objectInstance.Piece.GetPieceColour() == currentColour)
            {
                return false;
            }
            
            return true;
        }

        private static void TryFindMovesInDirection(
            int startingPositionX,
            int startingPositionY,
            int directionX,
            int directionY,
            MoveType moveType,
            bool mustBeSafe,
            int maxDistanceToSearch,
            IBoardLookup boardLookup,
            IBoardActionMask opponentActionMask,
            IList<Vector2Int> validMoves)
        {
            for (int i = 1; i <= maxDistanceToSearch; ++i)
            {
                int nextPositionX = startingPositionX + i * directionX;
                int nextPositionY = startingPositionY +i * directionY;
                
                // If this position is outside of our board bounds we stop searching immediately, no more valid moves exist in this direction
                if (!IsBoardPositionInBounds(nextPositionX, nextPositionY, boardLookup))
                {
                    break;
                }
                
                bool isContinousMovement = moveType == MoveType.Continuous;

                if (!IsValidBoardPosition(nextPositionX, nextPositionY, boardLookup))
                {
                    if (isContinousMovement)
                    {
                        // If this position is not valid and we're doing continuous movement we should finish searching
                        break;
                    }
                    
                    // We can continue looking as we're within board bounds and doing jump movement, this is just a hole in the board 
                    continue;
                }

                if (!IsVacantBoardPosition(nextPositionX, nextPositionY, boardLookup))
                {
                    if (isContinousMovement)
                    {
                        // If this board position is not vacant, no matter who is there and we're doing continuous movement, we should finish search 
                        break;
                    }

                    // We can continue looking as we're within board bounds and doing jump movement, this is just not a square we can move to
                    continue;
                }

                if (mustBeSafe && !IsSafeBoardPosition(nextPositionX, nextPositionY, opponentActionMask))
                {
                    if (isContinousMovement)
                    {
                        // If this board position is not safe and we're doing continuous movement, we should finish search 
                        break;
                    }

                    // We can continue looking as we're within board bounds and doing jump movement, this is just not a safe square we can move to
                    continue;
                }
                
                validMoves.Add(new Vector2Int(nextPositionX, nextPositionY));
            }
        }

        private static void TryFindAttackablePiecesInDirection(
            int startingPositionX,
            int startingPositionY,
            int directionX,
            int directionY,
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
                int nextPositionX = startingPositionX + i * directionX;
                int nextPositionY = startingPositionY + i * directionY;
                
                // If this position is outside of our board bounds we stop searching immediately, no more valid attackable pieces exist in this direction
                if (!IsBoardPositionInBounds(nextPositionX, nextPositionY, boardLookup))
                {
                    break;
                }
                
                bool isContinousMovement = moveType == MoveType.Continuous;

                if (!IsValidBoardPosition(nextPositionX, nextPositionY, boardLookup))
                {
                    if (isContinousMovement)
                    {
                        // If this position is not valid and we're doing continuous movement we should finish searching
                        break;
                    }
                    
                    // We can continue looking as we're within board bounds and doing jump movement, this is just a hole in the board 
                    continue;
                }

                if (mustBeSafe && !IsSafeBoardPosition(nextPositionX, nextPositionY, opponentActionMask))
                {
                    if (isContinousMovement)
                    {   
                        // If this board position is not safe and we're doing continuous movement, we should finish search 
                        break;
                    }

                    // We can continue looking as we're within board bounds and doing jump movement, this is just an invalid place to attack
                    continue;
                }
                
                // If this board position is vacant it's not a valid attack target, but it shouldn't stop our search
                if (IsVacantBoardPosition(nextPositionX, nextPositionY, boardLookup))
                {
                    continue;
                }

                BoardObjectInstance pieceToAttack = boardLookup.GetBoardObjectAt(nextPositionX, nextPositionY);
#if ULTRA_DEBUG_CHECKS
                UnityEngine.Debug.Assert(pieceToAttack != null, $"Despite checking that a piece is at ({nextPositionX}, {nextPositionY}), we've not managed to retrieve it when calculating attacks...");
#endif
                
                if (!IsAttackable(pieceToAttack, currentColour))
                {
                    if (isContinousMovement)
                    {
                        // There's a piece on this position, but we can't attack it so if we're doing continuous movement we should stop our search
                        break;
                    }

                    // We can continue looking as we're within board bounds and doing jump movement, this is just an invalid place to attack
                    continue;
                }

                validAttacks.Add(new Vector2Int(nextPositionX, nextPositionY));
            }
        }

        private static void TryFindThreatenedPositionsInDirection(
            int startingPositionX,
            int startingPositionY,
            int directionX,
            int directionY,
            MoveType moveType,
            int maxDistanceToSearch,
            IBoardLookup boardLookup,
            IList<Vector2Int> validThreats)
        {
            for (int i = 1; i <= maxDistanceToSearch; ++i)
            {
                int nextPositionX = startingPositionX + i * directionX;
                int nextPositionY = startingPositionY + i * directionY;

                // If this position is outside of our board bounds we stop searching immediately, no more valid threats exist in this direction
                if (!IsBoardPositionInBounds(nextPositionX, nextPositionY, boardLookup))
                {
                    break;
                }
                
                bool isContinousMovement = moveType == MoveType.Continuous;
                
                if (IsValidBoardPosition(nextPositionX, nextPositionY, boardLookup))
                {
                    // If it's a valid board position that we can reach via a move, it's a valid threatened square, even if there's a piece there (even if it's one of ours!)
                    validThreats.Add(new Vector2Int(nextPositionX, nextPositionY));
                }
                else if (isContinousMovement)
                {
                    // If this position is not valid and we're doing continuous movement we should finish searching
                    break;
                }

                // There's a piece on this position, so if we're doing continuous movement we have reached the limit of what we can threaten
                if (!IsVacantBoardPosition(nextPositionX, nextPositionY, boardLookup) && isContinousMovement)
                {
                    break;
                }
            }
        }
    }
}