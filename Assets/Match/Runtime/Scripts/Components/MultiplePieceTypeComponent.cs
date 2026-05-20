using System;
using System.ComponentModel;
using Celeste.Components;
using UnityEngine;

namespace QC.Match.Components
{
    [DisplayName("Multiple Piece Type")]
    public class MultiplePieceTypeComponent : ChessBoardObjectComponent, IChessPieceType
    {
        #region Save Data

        [Serializable]
        public class SaveData : ComponentData
        {
            public ChessPieceType currentPieceType;
        }
        
        #endregion
        
        #region Properties and Fields
        
        [SerializeField] private ChessPieceType startingPieceType = ChessPieceTypeExtensions.Everything;
        
        #endregion

        public override ComponentData CreateData()
        {
            return new SaveData
            {
                currentPieceType = startingPieceType
            };
        }

        public ChessPieceType GetPieceType(Instance instance)
        {
            return (instance.data as SaveData).currentPieceType;
        }
    }
}