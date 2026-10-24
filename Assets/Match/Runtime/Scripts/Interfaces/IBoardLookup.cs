using QC.Match.Objects;
using UnityEngine;

namespace QC.Match
{
    public interface IBoardLookup
    {
        bool IsPositionInBoard(int x, int y);
        bool DoesBoardTileExist(int x, int y);
        BoardTileInstance GetBoardTileAt(int x, int y);
        BoardObjectInstance GetBoardObjectAt(int x, int y);
    }
}