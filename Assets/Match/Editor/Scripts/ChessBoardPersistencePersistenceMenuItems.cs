using UnityEditor;
using Celeste.Persistence;
using CelesteEditor.Persistence;
using QC.Match.Managers;

namespace QCEditor.Match.Persistence
{
    public static class ChessBoardPersistencePersistenceMenuItems
    {
        [MenuItem("Quantum Chess/Save/Files/Open Chess Board Save", priority = 0)]
        public static void OpenChessBoardPersistenceSaveMenuItem()
        {
            PersistenceMenuItemUtility.OpenExplorerAtPersistentData();
        }

        [MenuItem("Quantum Chess/Save/Files/Delete Chess Board Save", priority = 100)]
        public static void DeleteChessBoardPersistenceSaveMenuItem()
        {
            PersistenceUtility.DeletePersistentDataFile(BoardManager.FILE_NAME);
        }
    }
}