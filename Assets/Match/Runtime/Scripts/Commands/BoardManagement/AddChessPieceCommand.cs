using Cysharp.Threading.Tasks;
using Lua;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public readonly struct AddChessPieceCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly int Guid;
        private readonly ChessPieceColour Colour;
        private readonly ChessPieceType Type;
        private readonly ChessPieceVerticalDirection VerticalDirection;
        private readonly Vector2Int Position;
        private readonly bool Interactable;

        public AddChessPieceCommand(
            int guid,
            ChessPieceColour colour,
            ChessPieceType type,
            ChessPieceVerticalDirection verticalDirection,
            Vector2Int position,
            bool interactable)
        {
            Guid = guid;
            Colour = colour;
            Type = type;
            VerticalDirection = verticalDirection;
            Position = position;
            Interactable = interactable;
        }
        
        public UniTask<CommandState> Execute(ChessBoardCommandArgs args)
        {
            ChessBoardObjectInstance instance = args.ChessBoard.CreateChessBoardObject(Guid);
            if (instance == null)
            {
                UnityEngine.Debug.LogError($"Failed to create chess board object with guid {Guid}.");
                return UniTask.FromResult(CommandState.Abandoned);
            }
            
            if (!instance.TryFindComponent<IChessPiece>(out var chessPiece))
            {
                UnityEngine.Debug.LogAssertion($"No {nameof(IChessPiece)} interface found on chess board object {Guid}.  Did you mean to try and add a chess piece?");
                return UniTask.FromResult(CommandState.Abandoned);
            }

            if (!instance.TryFindComponent<IGrid2DTransform>(out var gridTransform))
            {
                UnityEngine.Debug.LogAssertion($"No {nameof(IGrid2DTransform)} exists, this is a serious issue and should've been injected automatically, as it is a mandatory component.");
                return UniTask.FromResult(CommandState.Abandoned);
            }
            
            chessPiece.iFace.SetColour(chessPiece.instance, Colour);
            chessPiece.iFace.SetType(chessPiece.instance, Type);
            chessPiece.iFace.SetVerticalDirection(chessPiece.instance, VerticalDirection);
            gridTransform.iFace.SetPosition(gridTransform.instance, Position);

            if (instance.TryFindComponent<IInteractable>(out var interactable))
            {
                interactable.iFace.SetInteractable(interactable.instance, Interactable);
            }
            
            args.ChessBoard.AddChessBoardObjectInstance(instance);

            return UniTask.FromResult(CommandState.Completed);
        }
    }
}