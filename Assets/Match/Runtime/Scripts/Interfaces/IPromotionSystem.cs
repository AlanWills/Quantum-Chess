using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match
{
    public interface IPromotionSystem
    {
        bool CanPromote(BoardObjectInstance instance, IBoardLookup boardLookup, Vector2Int position);
        UniTask Promote(BoardObjectInstance instance, IBoardLookup boardLookup, Vector2Int position);
    }
}