using Cysharp.Threading.Tasks;
using QC.Match.Objects;

namespace QC.Match.Commands
{
    public static class InputCommandUtilities
    {
        public static UniTask<CommandState> SetInteractableForColour(
            Board board,
            PieceColour colour, 
            bool isInteractable)
        {
            foreach (var instance in board.Objects)
            {
                if (!instance.TryFindInterface<IPiece>(out var piece) ||
                    !instance.TryFindInterface<IInteractable>(out var interactable) ||
                    piece.GetPieceColour() != colour)
                {
                    continue;
                }
                
                interactable.SetInteractable(isInteractable);
            }

            return UniTask.FromResult(CommandState.Completed);
        }
        
        public static UniTask<CommandState> SetInteractableForNotColour(
            Board board,
            PieceColour colour, 
            bool isInteractable)
        {
            foreach (var instance in board.Objects)
            {
                if (!instance.TryFindInterface<IPiece>(out var piece) ||
                    !instance.TryFindInterface<IInteractable>(out var interactable) ||
                    piece.GetPieceColour() == colour)
                {
                    continue;
                }
                
                interactable.SetInteractable(isInteractable);
            }

            return UniTask.FromResult(CommandState.Completed);
        }
    }
}