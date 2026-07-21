namespace QC.Match.Players
{
    public readonly struct TileActionMask
    {
        public readonly bool CanPieceAttack;

        public TileActionMask(bool canPieceAttack)
        {
            CanPieceAttack = canPieceAttack;
        }
    }
}