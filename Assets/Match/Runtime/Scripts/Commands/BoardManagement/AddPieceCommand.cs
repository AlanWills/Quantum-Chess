using System.Threading;
using Cysharp.Threading.Tasks;
using Lua;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public readonly struct AddPieceCommand : ICommand<BoardCommandArgs>
    {
        private readonly int Guid;
        private readonly PieceVerticalDirection VerticalDirection;
        private readonly Vector2Int Position;
        private readonly bool Interactable;
        private readonly PieceColour? Colour;
        private readonly PieceType? Type;
        private readonly int? Value;

        public AddPieceCommand(
            int guid,
            PieceVerticalDirection verticalDirection,
            Vector2Int position,
            bool interactable)
        : this(guid, verticalDirection, position, interactable, null, null, null)
        {
        }

        public AddPieceCommand(
            int guid,
            PieceVerticalDirection verticalDirection,
            Vector2Int position,
            bool interactable,
            PieceColour? colour,
            PieceType? type,
            int? value)
        {
            Guid = guid;
            VerticalDirection = verticalDirection;
            Position = position;
            Interactable = interactable;
            Colour = colour;
            Type = type;
            Value = value;
        }
        
        public UniTask<CommandState> Execute(BoardCommandArgs args, CancellationToken token)
        {
            BoardObjectInstance instance = args.Board.CreateBoardObject(Guid);
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

            if (Colour.HasValue || Type.HasValue)
            {
                // We have specified type or colour rather than leave it to the preset, so we need a modifiable chess piece component
                if (!instance.TryFindComponent<IModifiableChessPiece>(out var modifiableChessPiece))
                {
                    UnityEngine.Debug.LogAssertion($"No {nameof(IModifiableChessPiece)} exists, but it looks like you're trying to customise colour and type of the chess piece you're creating.");
                    return UniTask.FromResult(CommandState.Abandoned);
                }

                if (Colour.HasValue)
                {
                    modifiableChessPiece.SetColour(Colour.Value);
                }

                if (Type.HasValue)
                {
                    modifiableChessPiece.SetType(Type.Value);
                }
            }

            if (Value.HasValue)
            {
                chessPiece.SetValue(Value.Value);
            }

            chessPiece.SetVerticalDirection(VerticalDirection);
            gridTransform.SetPosition(Position);
             
            if (instance.TryFindComponent<IInteractable>(out var interactable))
            {
                interactable.SetInteractable(Interactable);
            }
            
            args.Board.AddBoardObjectInstance(instance);

            return UniTask.FromResult(CommandState.Completed);
        }

        public override string ToString()
        {
            return $"{nameof(AddPieceCommand)} - Guid: {Guid} - Position: {Position}";
        }
    }
}