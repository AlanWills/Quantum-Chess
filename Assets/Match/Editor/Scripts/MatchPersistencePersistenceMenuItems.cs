using UnityEditor;
using Celeste.Persistence;
using CelesteEditor.Persistence;
using QC.Match.Managers;

namespace QCEditor.Match.Persistence
{
    public static class MatchPersistencePersistenceMenuItems
    {
        [MenuItem("Quantum Chess/Save/Files/Open Match Save", priority = 0)]
        public static void OpenMatchPersistenceSaveMenuItem()
        {
            PersistenceMenuItemUtility.OpenExplorerAtPersistentData();
        }

        [MenuItem("Quantum Chess/Save/Files/Delete Match Save", priority = 100)]
        public static void DeleteMatchPersistenceSaveMenuItem()
        {
            PersistenceUtility.DeletePersistentDataFile(MatchStateManager.FILE_NAME);
        }
    }
}