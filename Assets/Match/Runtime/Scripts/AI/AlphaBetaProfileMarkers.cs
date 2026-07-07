using Unity.Profiling;

namespace QC.Match.AI
{
    public static class AlphaBetaProfileMarkers
    {
        public static readonly ProfilerMarker MainLoop = new ProfilerMarker("Alpha Beta - Main Loop");
    }
}