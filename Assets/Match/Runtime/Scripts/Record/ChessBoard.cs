using System;
using System.Collections.Generic;
using Celeste.BoardGame.Runtime;
using Celeste.Events;
using Lua;
using UnityEngine;

namespace QC.Match.Record
{
    [LuaObject(kLuaName)]
    [CreateAssetMenu(fileName = nameof(ChessBoard), menuName = "QC/Match/Board State")]
    public partial class ChessBoard : ScriptableObject
    {
        #region Properties and Fields

        public const string kLuaName = nameof(ChessBoard);
        
        [SerializeField] private BoardGameObjectAddedEvent boardGameObjectAddedEvent;
        
        [NonSerialized] private readonly List<BoardGameObjectRuntime> currentBoardObjects = new(); 
        
        #endregion
    }
}