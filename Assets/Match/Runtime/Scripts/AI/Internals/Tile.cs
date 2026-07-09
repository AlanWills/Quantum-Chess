namespace QC.Match.AI.Internals
{
    public readonly struct Tile
    {
        public readonly bool IsValid;
        public readonly int Importance;
        public readonly Piece Piece;

        public Tile(bool isValid, int importance) :
            this(isValid, importance, default)
        {
        }
           
        public Tile(bool isValid, int importance, Piece piece)
        {
            IsValid = isValid;
            Importance = importance;
            Piece = piece;
        }

        public Tile WithoutPiece()
        {
            return new Tile(IsValid, Importance);
        }
           
        public Tile WithPiece(Piece piece)
        {
            return new Tile(IsValid, Importance, piece);
        }
    }
}