using QC.Match.Objects;

namespace QC.Match.Events
{
    public readonly struct ChessBoardObjectSelectionChangedArgs
    {
        public readonly ChessBoardObjectInstance PreviousSelectedObject;
        public readonly ChessBoardObjectInstance NewSelectedObject;
        
        public ChessBoardObjectSelectionChangedArgs(
            ChessBoardObjectInstance previousObject,
            ChessBoardObjectInstance newObject)
        {
            PreviousSelectedObject = previousObject;
            NewSelectedObject = newObject;
        }
    }
}