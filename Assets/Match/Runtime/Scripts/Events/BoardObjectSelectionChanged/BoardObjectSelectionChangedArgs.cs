using QC.Match.Objects;

namespace QC.Match.Events
{
    public readonly struct BoardObjectSelectionChangedArgs
    {
        public readonly BoardObjectInstance PreviousSelectedObject;
        public readonly BoardObjectInstance NewSelectedObject;
        
        public BoardObjectSelectionChangedArgs(
            BoardObjectInstance previousObject,
            BoardObjectInstance newObject)
        {
            PreviousSelectedObject = previousObject;
            NewSelectedObject = newObject;
        }
    }
}