using System;
using Celeste.Components;
using QC.Match.Components;

namespace QC.Match.Objects
{
    public class ChessBoardObjectInstance : ComponentContainerInstance<ChessBoardObjectComponent>
    {
        #region Properties and Fields

        public int Guid => chessBoardObject.Guid;
        
        [NonSerialized] private readonly ChessBoardObject chessBoardObject;

        #endregion

        public ChessBoardObjectInstance(ChessBoardObject chessBoardObject)
        {
            this.chessBoardObject = chessBoardObject;
        }
    }
}