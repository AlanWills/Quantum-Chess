namespace QC.Match.AI.Internals
{
    public readonly struct TileBlit
    {
        public readonly bool IsValid;
        public readonly int Importance;
        public readonly Piece Piece;

        public TileBlit(bool isValid, int importance) :
            this(isValid, importance, default)
        {
        }
           
        public TileBlit(bool isValid, int importance, Piece piece)
        {
            IsValid = isValid;
            Importance = importance;
            Piece = piece;
        }

        public TileBlit WithoutPiece()
        {
            return new TileBlit(IsValid, Importance);
        }
           
        public TileBlit WithPiece(Piece piece)
        {
            return new TileBlit(IsValid, Importance, piece);
        }
    }
}