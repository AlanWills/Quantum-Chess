using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public static class MoveCommandUtils
    {
        public static CommandState TryMoveChessBoardObject(
            ChessBoardCommandArgs args,
            ChessBoardObjectInstance instance,
            Vector2Int endingPosition)
        {
            if (instance == null)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to move a null chess board object when executing move command.");
                return CommandState.Abandoned;
            }
            
            if (!instance.TryFindComponent<IGrid2DTransform>(out var grid2DTransform))
            {
                UnityEngine.Debug.LogAssertion("Could not find transform component for chess board object when executing move object command.");
                return CommandState.Abandoned;
            }
            
            ChessBoardObjectInstance endingPositionInstance = args.ChessBoard.GetChessBoardObjectAt(endingPosition);
            
            if (endingPositionInstance == null)
            {
                grid2DTransform.iFace.SetPosition(grid2DTransform.instance, endingPosition);
                return CommandState.Completed;
            }
            
            bool instanceHasChessPiece = instance.TryFindComponent<IChessPiece>(out var instanceChessPiece);
            bool endingPositionInstanceHasChessPiece = endingPositionInstance.TryFindComponent<IChessPiece>(out var endingPositionInstanceChessPiece);

            switch (instanceHasChessPiece)
            {
                case true when endingPositionInstanceHasChessPiece:
                    if (instanceChessPiece.iFace.GetPieceColour(instanceChessPiece.instance) ==
                        endingPositionInstanceChessPiece.iFace.GetPieceColour(endingPositionInstanceChessPiece
                            .instance))
                    {
                        // Trying to move over a piece with the same colour
                        // We do not allow this
                        return CommandState.Abandoned;
                    }
                    
                    grid2DTransform.iFace.SetPosition(grid2DTransform.instance, endingPosition);
                    args.ChessBoard.RemoveChessBoardObject(endingPositionInstance);
                    
                    return CommandState.Completed;
                case true:
                    // We're moving a chess piece to a place with another object that is not a chess piece
                    // Not sure what to do here just yet, but for now let's prevent it
                    return CommandState.Abandoned;
                case false when !endingPositionInstanceHasChessPiece:
                    // Neither object is a chess piece
                    // Not sure what to do here just yet, but for now let's prevent it
                    return CommandState.Abandoned;
            }

            return CommandState.Abandoned;
        }
    }
}