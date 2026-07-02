using System.Runtime.CompilerServices;
using Celeste.Components;

namespace QC.Match
{
    public interface IObjectStatus
    {
        void SetHasMoved(Instance instance, bool hasMoved);
    }

    public static class ObjectStatusExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetHasMoved(this InterfaceHandle<IObjectStatus> objectStatus, bool hasMoved)
        {
            objectStatus.iFace.SetHasMoved(objectStatus.instance, hasMoved);
        }
    }
}