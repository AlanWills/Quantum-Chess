using UnityEngine;
using Celeste.Persistence;
using QC.Match.Persistence;
using QC.Match.Objects;

namespace QC.Match.Managers
{
    [AddComponentMenu("QC/Match/Chess Board Manager")]
    public class ChessBoardManager : PersistentSceneManager<ChessBoardManager, ChessBoardDTO>
    {
		#region Properties and Fields
	
        public const string FILE_NAME = "chess_board.dat";
		
        protected override string FileName => FILE_NAME;

        [SerializeField] private ChessBoard chessBoard;

		#endregion
		
		#region Unity Methods

		protected override void OnDestroy()
		{
			chessBoard.Shutdown();
			
			base.OnDestroy();
		}

		#endregion
		
		#region Save/Load

        protected override ChessBoardDTO Serialize()
        {
	        return chessBoard.Serialize();
        }

        protected override void Deserialize(ChessBoardDTO dto)
        {
	        chessBoard.Initialize(dto);
        }

        protected override void SetDefaultValues()
        {
	        chessBoard.Initialize();
        }
		
		#endregion
    }
}