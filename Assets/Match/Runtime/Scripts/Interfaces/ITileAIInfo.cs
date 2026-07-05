using Celeste.Components;

namespace QC.Match
{
    public interface ITileAIInfo
    {
        int GetImportance(Instance instance);
        void SetImportance(Instance instance, int importance);
    }

    public static class TileAIInfoExtensions
    {
        public static int GetImportance(this InterfaceHandle<ITileAIInfo> tileAIInfo)
        {
            return tileAIInfo.iFace.GetImportance(tileAIInfo.instance);
        }
        
        public static void SetImportance(this InterfaceHandle<ITileAIInfo> tileAIInfo, int importance)
        {
            tileAIInfo.iFace.SetImportance(tileAIInfo.instance, importance);
        }
    }
}