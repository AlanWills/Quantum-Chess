using QC.Match.Objects;
using UnityEngine;

namespace QC.Match
{
    public interface IBoardLookup
    {
        bool DoesBoardTileExist(Vector2Int boardPosition);
        BoardObjectInstance GetBoardObjectAt(Vector2Int boardPosition);
    }
}