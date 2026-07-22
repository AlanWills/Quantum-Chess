using UnityEngine;

namespace QC.Match
{
    public interface IPlayerActionMaskLookup
    {
        bool DoesPlayerHavePieceAbleToAttack(ChessPieceColour playerColour, Vector2Int boardPosition);
    }
}