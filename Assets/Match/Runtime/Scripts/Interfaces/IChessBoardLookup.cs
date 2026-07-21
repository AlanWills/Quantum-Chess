using QC.Match.Objects;
using UnityEngine;

namespace QC.Match
{
    public interface IChessBoardLookup
    {
        bool DoesChessBoardTileExist(Vector2Int boardPosition);
        ChessBoardObjectInstance GetChessBoardObjectAt(Vector2Int boardPosition);
    }
}