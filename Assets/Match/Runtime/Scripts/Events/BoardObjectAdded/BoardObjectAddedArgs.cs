using QC.Match.Objects;

namespace QC.Match.Events
{
    public readonly struct BoardObjectAddedArgs
    {
        public readonly BoardObjectInstance AddedObject;

        public BoardObjectAddedArgs(BoardObjectInstance addedObject)
        {
            AddedObject = addedObject;
        }
    }
}