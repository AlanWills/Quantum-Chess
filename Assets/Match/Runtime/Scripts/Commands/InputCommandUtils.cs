using QC.Match.Objects;

namespace QC.Match.Commands
{
    public static class InputCommandUtils
    {
        public static CommandState SetInteractableForColour(
            ChessBoard chessBoard,
            ChessPieceColour colour, 
            bool isInteractable)
        {
            foreach (var instance in chessBoard.Objects)
            {
                if (!instance.TryFindComponent<IChessPiece>(out var piece) ||
                    !instance.TryFindComponent<IInteractable>(out var interactable) ||
                    piece.iFace.GetPieceColour(piece.instance) != colour)
                {
                    continue;
                }
                
                interactable.iFace.SetInteractable(interactable.instance, isInteractable);
            }

            return CommandState.Completed;
        }
        
        public static CommandState SetInteractableForNotColour(
            ChessBoard chessBoard,
            ChessPieceColour colour, 
            bool isInteractable)
        {
            foreach (var instance in chessBoard.Objects)
            {
                if (!instance.TryFindComponent<IChessPiece>(out var piece) ||
                    !instance.TryFindComponent<IInteractable>(out var interactable) ||
                    piece.iFace.GetPieceColour(piece.instance) == colour)
                {
                    continue;
                }
                
                interactable.iFace.SetInteractable(interactable.instance, isInteractable);
            }

            return CommandState.Completed;
        }
    }
}