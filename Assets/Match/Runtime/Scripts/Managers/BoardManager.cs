using UnityEngine;
using Celeste.Persistence;
using QC.Match.Persistence;
using QC.Match.Objects;

namespace QC.Match.Managers
{
    [AddComponentMenu("QC/Match/Chess Board Manager")]
    public class BoardManager : PersistentSceneManager<BoardManager, BoardDTO>
    {
		#region Properties and Fields
	
        public const string FILE_NAME = "board.dat";
		
        protected override string FileName => FILE_NAME;

        [SerializeField] private Board board;

		#endregion
		
		#region Unity Methods

		protected override void OnDestroy()
		{
			board.Shutdown();
			
			base.OnDestroy();
		}

		#endregion
		
		#region Save/Load

        protected override BoardDTO Serialize()
        {
	        return board.Serialize();
        }

        protected override void Deserialize(BoardDTO dto)
        {
	        board.Initialize(dto);
        }

        protected override void SetDefaultValues()
        {
	        board.Initialize();
        }
		
		#endregion
		
		#region Callbacks

		public void OnBeforeScenarioSetUp()
		{
			board.Clear();
		}
		
		#endregion
    }
}