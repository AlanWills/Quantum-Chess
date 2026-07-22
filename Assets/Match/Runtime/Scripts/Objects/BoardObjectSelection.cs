using System;
using Lua;
using QC.Match.Events;
using UnityEngine;

namespace QC.Match.Objects
{
    [LuaObject(kLuaName)]
    [CreateAssetMenu(fileName = nameof(BoardObjectSelection), menuName = "QC/Match/Board Object Selection")]
    public partial class BoardObjectSelection : ScriptableObject
    {
        #region Properties and Fields

        public const string kLuaName = nameof(BoardObjectSelection);
        
        [LuaMember("hasSelectedObject")] public bool HasSelectedObject => CurrentSelectedObject != null;
        
        public BoardObjectInstance CurrentSelectedObject
        {
            get => currentSelectedObject;
            set
            {
                if (currentSelectedObject != value)
                {
                    BoardObjectInstance previous = currentSelectedObject;
                    currentSelectedObject = value;
                    
                    currentSelectedObjectChanged?.Invoke(new BoardObjectSelectionChangedArgs(previous, currentSelectedObject));
                }
            }
        }
        
        [SerializeField] private BoardObjectSelectionChangedEvent currentSelectedObjectChanged;
        
        [NonSerialized] private BoardObjectInstance currentSelectedObject;
        
        #endregion
    }
}