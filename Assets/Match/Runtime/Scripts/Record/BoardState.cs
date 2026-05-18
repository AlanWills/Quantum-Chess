using System;
using System.Collections.Generic;
using Celeste.BoardGame.Runtime;
using Lua;
using UnityEngine;

namespace QC.Match.Record
{
    [LuaObject(kLuaName)]
    [CreateAssetMenu(fileName = nameof(BoardState), menuName = "QC/Match/Board State")]
    public partial class BoardState : ScriptableObject
    {
        #region Properties and Fields

        public const string kLuaName = nameof(BoardState);
        
        [NonSerialized] private readonly List<BoardGameObjectRuntime> currentBoardObjects = new(); 
        
        #endregion
    }
}