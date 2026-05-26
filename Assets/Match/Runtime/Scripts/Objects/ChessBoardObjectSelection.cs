using System;
using Lua;
using QC.Match.Events;
using UnityEngine;

namespace QC.Match.Objects
{
    [LuaObject(kLuaName)]
    [CreateAssetMenu(fileName = nameof(ChessBoardObjectSelection), menuName = "QC/Match/Chess Board Object Selection")]
    public partial class ChessBoardObjectSelection : ScriptableObject
    {
        #region Properties and Fields

        public const string kLuaName = nameof(ChessBoardObjectSelection);
        
        [LuaMember("hasSelectedObject")] public bool HasSelectedObject => CurrentSelectedObject != null;
        
        public ChessBoardObjectInstance CurrentSelectedObject
        {
            get => currentSelectedObject;
            set
            {
                if (currentSelectedObject != value)
                {
                    ChessBoardObjectInstance previous = currentSelectedObject;
                    currentSelectedObject = value;
                    
                    currentSelectedObjectChanged?.Invoke(new ChessBoardObjectSelectionChangedArgs(previous, currentSelectedObject));
                }
            }
        }
        
        [SerializeField] private ChessBoardObjectSelectionChangedEvent currentSelectedObjectChanged;
        
        [NonSerialized] private ChessBoardObjectInstance currentSelectedObject;
        
        #endregion
    }
}