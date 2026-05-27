using QC.Match.Objects;

namespace QC.Match.Events
{
    public readonly struct ChessBoardObjectAddedArgs
    {
        public readonly ChessBoardObjectInstance AddedObject;

        public ChessBoardObjectAddedArgs(ChessBoardObjectInstance addedObject)
        {
            AddedObject = addedObject;
        }
    }
}