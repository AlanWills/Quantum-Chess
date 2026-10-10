using QC.Match.Objects;
using UnityEngine;

namespace QC.Match
{
    public interface IBoardLookup
    {
        bool IsPositionInBoard(Vector2Int boardPosition);
        bool DoesBoardTileExist(Vector2Int boardPosition);
        BoardTileInstance GetBoardTileAt(Vector2Int boardTilePosition);
        BoardObjectInstance GetBoardObjectAt(Vector2Int boardPosition);
    }
}