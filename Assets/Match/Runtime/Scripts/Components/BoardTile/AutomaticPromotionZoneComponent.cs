using System;
using System.Collections.Generic;
using System.ComponentModel;
using Celeste.Components;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace QC.Match.Components
{
    [DisplayName("Automatic Promotion Zone")]
    [CreateAssetMenu(fileName = nameof(AutomaticPromotionZoneComponent), menuName = "QC/Match/Components/Automatic Promotion Zone")]
    public class AutomaticPromotionZoneComponent : BoardTileComponent, IPromotionZone
    {
        #region Save Data

        [Serializable]
        private class SaveData : ComponentData
        {
            public List<Promotion> possiblePromotions = new();
        }
        
        #endregion

        #region Properties and Fields

        [SerializeField] private List<Promotion> startingPossiblePromotions = new();

        #endregion

        public override ComponentData CreateData()
        {
            return new SaveData
            {
                possiblePromotions = new List<Promotion>(startingPossiblePromotions)
            };
        }

        public void SetPromotions(Instance instance, IReadOnlyList<Promotion> promotions)
        {
            SaveData saveData = instance.data as SaveData;
            saveData.possiblePromotions.Clear();
            saveData.possiblePromotions.AddRange(promotions);
            instance.events.ComponentDataChanged.Invoke();
        }

        public bool CanPromote(Instance instance, InterfaceHandle<IPiece> piece)
        {
            SaveData saveData = instance.data as SaveData;
            return GetMatchingPromotionIndex(saveData, piece) >= 0;
        }

        public UniTask Promote(Instance instance, InterfaceHandle<IPiece> piece)
        {
            UnityEngine.Debug.Assert(CanPromote(instance, piece), "Attempting to promote a piece that cannot be promoted!");
            SaveData saveData = instance.data as SaveData;
            int promotionIndex = GetMatchingPromotionIndex(saveData, piece);
            Promotion promotion = saveData.possiblePromotions[promotionIndex];
            
            piece.SetType(promotion.promotedPieceType);

            return UniTask.CompletedTask;
        }

        private int GetMatchingPromotionIndex(SaveData saveData, InterfaceHandle<IPiece> piece)
        {
            PieceType pieceType = piece.GetPieceType();
            PieceColour pieceColour = piece.GetPieceColour();

            for (int i = 0, n = saveData.possiblePromotions.Capacity; i < n; ++i)
            {
                Promotion promotion = saveData.possiblePromotions[i];
                bool pieceTypeMatches = promotion.requiredPieceType == PieceType.None ||
                                        promotion.requiredPieceType == pieceType;
                bool pieceColourMatches = promotion.requiredPieceColour == PieceColour.None ||
                                          promotion.requiredPieceColour == pieceColour;

                if (pieceTypeMatches && pieceColourMatches)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}