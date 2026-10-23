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
        public InterfaceHandle<IGrid2DTransform> Grid2DTransform => grid2DTransform;
        public InterfaceHandle<IPiece> Piece => piece;
        public InterfaceHandle<IActionProvider> ActionProvider => actionProvider;
        
        [NonSerialized] private readonly BoardObject boardObject;
        [NonSerialized] private readonly InterfaceHandle<IGrid2DTransform> grid2DTransform;
        [NonSerialized] private readonly InterfaceHandle<IPiece> piece;
        [NonSerialized] private readonly InterfaceHandle<IActionProvider> actionProvider;

        #endregion

        public BoardObjectInstance(BoardObject boardObject)
        {
            this.boardObject = boardObject;
            
            InitializeComponents(boardObject);

            bool foundTransform = TryFindInterface(out grid2DTransform);
            UnityEngine.Debug.Assert(foundTransform, $"Failed to find {nameof(IGrid2DTransform)} on {nameof(BoardObjectInstance)}.  This is a serious issue!");

            TryFindInterface(out piece);
            TryFindInterface(out actionProvider);
        }
    }
}