using System;
using Celeste.Components;
using Lua;
using QC.Match.Components;

namespace QC.Match.Objects
{
    public class BoardObjectInstance : ComponentContainerInstance<BoardObjectComponent>
    {
        #region Properties and Fields

        public int Guid => boardObject.Guid;
        
        [NonSerialized] private readonly BoardObject boardObject;

        #endregion

        public BoardObjectInstance(BoardObject boardObject)
        {
            this.boardObject = boardObject;
            
            InitializeComponents(boardObject);
        }
    }
}