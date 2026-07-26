namespace QC.Match
{
    public interface IBoardActionMaskLookup
    {
        IBoardActionMask GetBoardActionMaskForPlayer(int playerIndex);
        IBoardActionMask GetOpponentBoardActionMaskForPlayer(int playerIndex);
    }
}