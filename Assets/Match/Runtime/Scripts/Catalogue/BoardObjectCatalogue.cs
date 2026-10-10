using UnityEngine;
using Celeste.Objects;
using QC.Match.Objects;

namespace QC.Match.Catalogue
{
    [CreateAssetMenu(fileName = nameof(BoardObjectCatalogue), menuName = "QC/Match/Objects/Board Object Catalogue")]
    public class BoardObjectCatalogue : ListScriptableObject<BoardObject>, IAutomaticImportSettings
    {
        #region Properties and Fields

        public AutomaticImportBehaviour ImportBehaviour => importBehaviour;
        
        [SerializeField] private AutomaticImportBehaviour importBehaviour;
        
        #endregion
        
        public BoardObject MustFindItemByGuid(int guid)
        {
            BoardObject boardObject = FindItemByGuid(guid);
            UnityEngine.Debug.Assert(boardObject != null, $"Failed to find {nameof(BoardObject)} with guid '{guid}' in {name}.");
            return boardObject;
        }
        
        public BoardObject FindItemByGuid(int guid)
        {
            return FindItem(x => x.Guid == guid);
        }
    }
}