using UnityEditor;
using Celeste.Persistence;
using CelesteEditor.Persistence;
using QC.Campaign.Managers;

namespace QCEditor.Campaign.Persistence
{
    public static class CampaignPersistencePersistenceMenuItems
    {
        [MenuItem("Quantum Chess/Save/Files/Open Campaign Save", priority = 0)]
        public static void OpenCampaignPersistenceSaveMenuItem()
        {
            PersistenceMenuItemUtility.OpenExplorerAtPersistentData();
        }

        [MenuItem("Quantum Chess/Save/Files/Delete Campaign Save", priority = 100)]
        public static void DeleteCampaignPersistenceSaveMenuItem()
        {
            PersistenceUtility.DeletePersistentDataFile(CampaignManager.FILE_NAME);
        }
    }
}