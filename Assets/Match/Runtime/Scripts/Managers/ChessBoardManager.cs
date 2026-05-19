using UnityEngine;
using Celeste.Persistence;
using QC.Match.Persistence;

namespace QC.Match.Managers
{
    [AddComponentMenu("QC/Match/Board State Manager")]
    public class ChessBoardManager : PersistentSceneManager<ChessBoardManager, ChessBoardManagerDTO>
    {
		#region Properties and Fields
	
        public const string FILE_NAME = "ChessBoardManager.dat";
		
        protected override string FileName => FILE_NAME;

		#endregion
		
		#region Save/Load

        protected override ChessBoardManagerDTO Serialize()
        {
            return new ChessBoardManagerDTO();
        }

        protected override void Deserialize(ChessBoardManagerDTO dto)
        {
        }

        protected override void SetDefaultValues()
        {
        }
		
		#endregion
    }
}