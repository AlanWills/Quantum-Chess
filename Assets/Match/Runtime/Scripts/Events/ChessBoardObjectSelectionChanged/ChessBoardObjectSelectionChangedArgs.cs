using QC.Match.Objects;

namespace QC.Match.Events
{
    public readonly struct ChessBoardObjectSelectionChangedArgs
    {
        public readonly ChessBoardObjectInstance previousSelectedObject;
        public readonly ChessBoardObjectInstance newSelectedObject;
        
        public ChessBoardObjectSelectionChangedArgs(
            ChessBoardObjectInstance previousObject,
            ChessBoardObjectInstance newObject)
        {
            previousSelectedObject = previousObject;
            newSelectedObject = newObject;
        }
    }
}