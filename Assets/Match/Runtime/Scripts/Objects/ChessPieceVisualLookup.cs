using System;
using System.Collections.Generic;
using UnityEngine;

namespace QC.Match.Objects
{
    [CreateAssetMenu(fileName = nameof(ChessPieceVisualLookup), menuName = "QC/Match/Objects/Chess Piece Visual Lookup")]
    public class ChessPieceVisualLookup : ScriptableObject
    {
        #region Piece Type, Colour And Sprite

        [Serializable]
        private struct PieceTypeColourAndSprite
        {
            public PieceType Type;
            public PieceColour Colour;
            public Sprite Sprite;
        }
        
        #endregion
        
        #region Properties and Fields

        [SerializeField] private List<PieceTypeColourAndSprite> sprites = new();
        [SerializeField] private Sprite fallbackSprite;

        #endregion

        public Sprite GetSprite(PieceColour colour, PieceType type)
        {
            if (colour == PieceColour.Unknown)
            {
                return fallbackSprite;
            }

            if (type == PieceType.None)
            {
                return fallbackSprite;
            }
            
            int index = sprites.FindIndex(sprite => sprite.Type == type && sprite.Colour == colour);
            UnityEngine.Debug.Assert(index >= 0, $"Failed to find sprite for colour {colour} and type {type}.");
            return index >= 0 ? sprites[index].Sprite : fallbackSprite;
        }
    }
}