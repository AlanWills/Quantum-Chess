using QC.Match.Objects;

namespace QC.Match.Events
{
    public readonly struct ChessBoardObjectRemovedArgs
    {
        public readonly ChessBoardObjectInstance RemovedInstance;

        public ChessBoardObjectRemovedArgs(ChessBoardObjectInstance removedInstance)
        {
            RemovedInstance = removedInstance;
        }
    }
}