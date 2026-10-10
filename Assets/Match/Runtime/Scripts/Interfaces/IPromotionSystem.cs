using Cysharp.Threading.Tasks;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match
{
    public interface IPromotionSystem
    {
        bool CanPromote(
            BoardObjectInstance instance, 
            Vector2Int position,
            IBoardLookup boardLookup);
        UniTask<BoardObjectInstance> Promote(
            BoardObjectInstance instance, 
            Vector2Int position,
            Board board);
    }
}