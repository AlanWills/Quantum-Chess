using QC.Match.Objects;

namespace QC.Match.Events
{
    public readonly struct BoardObjectRemovedArgs
    {
        public readonly BoardObjectInstance RemovedInstance;

        public BoardObjectRemovedArgs(BoardObjectInstance removedInstance)
        {
            RemovedInstance = removedInstance;
        }
    }
}