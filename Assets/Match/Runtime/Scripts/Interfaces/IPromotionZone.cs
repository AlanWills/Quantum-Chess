using System;
using Celeste.Components;
using Cysharp.Threading.Tasks;

namespace QC.Match
{
    [Serializable]
    public struct Promotion
    {
        public PieceType requiredPieceType;
        public PieceColour requiredPieceColour;
        public PieceType promotedPieceType;
    }
    
    public interface IPromotionZone
    {
        bool CanPromote(Instance instance, InterfaceHandle<IPiece> piece);
        UniTask Promote(Instance instance, InterfaceHandle<IPiece> piece);
    }
}