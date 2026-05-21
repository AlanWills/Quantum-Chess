using UnityEditor;
using Celeste.Persistence;
using CelesteEditor.Persistence;
using QC.Match.Managers;

namespace QCEditor.Match.Persistence
{
    public static class BoardStatePersistencePersistenceMenuItems
    {
        [MenuItem("Quantum Chess/Save/Files/Open Board State Save", priority = 0)]
        public static void OpenBoardStatePersistenceSaveMenuItem()
        {
            PersistenceMenuItemUtility.OpenExplorerAtPersistentData();
        }

        [MenuItem("Quantum Chess/Save/Files/Delete Board State Save", priority = 100)]
        public static void DeleteBoardStatePersistenceSaveMenuItem()
        {
            PersistenceUtility.DeletePersistentDataFile(ChessBoardManager.FILE_NAME);
        }
    }
}