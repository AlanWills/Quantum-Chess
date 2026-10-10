using QC.Match.Objects;

namespace QC.Match.AI.Internals
{
    public readonly struct TileBlit
    {
        public readonly bool IsValid;
        public readonly int Importance;
        public readonly BoardTileInstance Instance;
        public readonly Piece Piece;

        public TileBlit(BoardTileInstance instance, bool isValid, int importance) :
            this(instance, isValid, importance, default)
        {
        }
           
        public TileBlit(BoardTileInstance instance, bool isValid, int importance, Piece piece)
        {
            Instance = instance;
            IsValid = isValid;
            Importance = importance;
            Piece = piece;
        }

        public TileBlit WithoutPiece()
        {
            return new TileBlit(Instance, IsValid, Importance);
        }
           
        public TileBlit WithPiece(Piece piece)
        {
            return new TileBlit(Instance, IsValid, Importance, piece);
        }
    }
}