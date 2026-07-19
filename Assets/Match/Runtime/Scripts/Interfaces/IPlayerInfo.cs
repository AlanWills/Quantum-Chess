namespace QC.Match
{
    public interface IPlayerInfo
    {
        ChessPieceColour GetPlayerColour(int playerIndex);
        int GetNumActionsTakenByActivePlayer();
    }
}