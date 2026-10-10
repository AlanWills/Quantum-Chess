using Cysharp.Threading.Tasks;
using UnityEngine;

namespace QC.Match.Objects
{
    [CreateAssetMenu(fileName = nameof(PromotionSystem), menuName = "QC/Match/Promotion System")]
    public class PromotionSystem : ScriptableObject, IPromotionSystem
    {
        public bool CanPromote(BoardObjectInstance instance, IBoardLookup boardLookup, Vector2Int position)
        {
            if (!instance.TryFindInterface<IPiece>(out var piece))
            {
                return false;
            }
            
            if (!boardLookup.DoesBoardTileExist(position))
            {
                return false;
            }

            BoardTileInstance boardTileInstance = boardLookup.GetBoardTileAt(position);
            if (!boardTileInstance.TryFindInterface<IPromotionZone>(out var promotionZone))
            {
                return false;
            }

            return promotionZone.iFace.CanPromote(promotionZone.instance, piece);
        }

        public UniTask Promote(BoardObjectInstance instance, IBoardLookup boardLookup, Vector2Int position)
        {
            UnityEngine.Debug.Assert(CanPromote(instance, boardLookup, position), 
                $"Attempting to promote a {nameof(BoardObjectInstance)} via the {nameof(PromotionSystem)}, but it is not eligible for promotion.");
            
            BoardTileInstance boardTileInstance = boardLookup.GetBoardTileAt(position);
            
            boardTileInstance.TryFindInterface<IPromotionZone>(out var promotionZone);
            instance.TryFindInterface<IPiece>(out var piece);

            return promotionZone.iFace.Promote(promotionZone.instance, piece);
        }
    }
}