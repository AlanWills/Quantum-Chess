using Cysharp.Threading.Tasks;
using QC.Match.Catalogue;
using UnityEngine;

namespace QC.Match.Objects
{
    [CreateAssetMenu(fileName = nameof(PromotionSystem), menuName = "QC/Match/Promotion System")]
    public class PromotionSystem : ScriptableObject, IPromotionSystem
    {
        public bool CanPromote(BoardObjectInstance instance, Vector2Int position, IBoardLookup boardLookup)
        {
            if (!instance.Piece.IsValid)
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

            return promotionZone.iFace.CanPromote(promotionZone.instance, instance.Piece);
        }

        public async UniTask<BoardObjectInstance> Promote(
            BoardObjectInstance instance, 
            Vector2Int position, 
            Board board)
        {
            UnityEngine.Debug.Assert(CanPromote(instance, position, board), 
                $"Attempting to promote a {nameof(BoardObjectInstance)} via the {nameof(PromotionSystem)}, but it is not eligible for promotion.");
            
            BoardTileInstance boardTileInstance = board.GetBoardTileAt(position);
            
            boardTileInstance.TryFindInterface<IPromotionZone>(out var promotionZone);

            int newPieceGuid = await promotionZone.iFace.Promote(promotionZone.instance, instance.Piece);
            BoardObjectInstance promotedInstance = board.CreateBoardObject(newPieceGuid);

            if (promotedInstance == null)
            {
                UnityEngine.Debug.LogAssertion($"Failed to create promoted board object with guid {newPieceGuid}.");
                return null;
            }

            if (instance.TryFindInterface<IInteractable>(out var originalInteractable) &&
                promotedInstance.TryFindInterface<IInteractable>(out var newInteractable))
            {
                newInteractable.SetInteractable(originalInteractable.IsInteractable());
            }

            if (promotedInstance.Grid2DTransform.IsValid)
            {
                promotedInstance.Grid2DTransform.SetPosition(position);
            }

            board.RemoveBoardObject(instance);
            board.AddBoardObjectInstance(promotedInstance);
            
            return promotedInstance;
        }
    }
}