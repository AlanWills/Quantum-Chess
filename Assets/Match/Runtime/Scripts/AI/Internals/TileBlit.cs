using QC.Match.Objects;

namespace QC.Match.AI.Internals
{
    public struct TileBlit
    {
        public readonly bool IsValid;
        public readonly int Importance;
        public readonly BoardTileInstance Instance;
        public Piece Piece;

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
    }
}