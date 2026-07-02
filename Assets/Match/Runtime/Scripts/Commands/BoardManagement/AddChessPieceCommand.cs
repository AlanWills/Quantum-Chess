using Cysharp.Threading.Tasks;
using Lua;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public readonly struct AddChessPieceCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly int Guid;
        private readonly ChessPieceVerticalDirection VerticalDirection;
        private readonly Vector2Int Position;
        private readonly bool Interactable;
        private readonly ChessPieceColour Colour;
        private readonly ChessPieceType Type;

        public AddChessPieceCommand(
            int guid,
            ChessPieceVerticalDirection verticalDirection,
            Vector2Int position,
            bool interactable)
        : this(guid, verticalDirection, position, interactable, ChessPieceColour.Unknown, ChessPieceType.None)
        {
        }

        public AddChessPieceCommand(
            int guid,
            ChessPieceVerticalDirection verticalDirection,
            Vector2Int position,
            bool interactable,
            ChessPieceColour colour,
            ChessPieceType type)
        {
            Guid = guid;
            VerticalDirection = verticalDirection;
            Position = position;
            Interactable = interactable;
            Colour = colour;
            Type = type;
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

            if (Colour != ChessPieceColour.Unknown && Type != ChessPieceType.None)
            {
                // We have specified type and colour rather than leave it to the preset, so we need a modifiable chess piece component
                if (!instance.TryFindComponent<IModifiableChessPiece>(out var modifiableChessPiece))
                {
                    UnityEngine.Debug.LogAssertion($"No {nameof(IModifiableChessPiece)} exists, but it looks like you're trying to customise colour and type of the chess piece you're creating.");
                    return UniTask.FromResult(CommandState.Abandoned);
                }
                
                modifiableChessPiece.SetColour(Colour);
                modifiableChessPiece.SetType(Type);
            }

            chessPiece.SetVerticalDirection(VerticalDirection);
            gridTransform.SetPosition(Position);
             
            if (instance.TryFindComponent<IInteractable>(out var interactable))
            {
                interactable.SetInteractable(Interactable);
            }
            
            args.ChessBoard.AddChessBoardObjectInstance(instance);

            return UniTask.FromResult(CommandState.Completed);
        }
    }
}