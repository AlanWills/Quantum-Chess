using UnityEngine;
using Celeste.Persistence;
using QC.Match.Persistence;

namespace QC.Match.Managers
{
    [AddComponentMenu("QC/Match/Board State Manager")]
    public class BoardStateManager : PersistentSceneManager<BoardStateManager, BoardStateManagerDTO>
    {
		#region Properties and Fields
	
        public const string FILE_NAME = "BoardStateManager.dat";
		
        protected override string FileName => FILE_NAME;

		#endregion
		
		#region Save/Load

        protected override BoardStateManagerDTO Serialize()
        {
            return new BoardStateManagerDTO();
        }

        protected override void Deserialize(BoardStateManagerDTO dto)
        {
        }

        protected override void SetDefaultValues()
        {
        }
		
		#endregion
    }
}