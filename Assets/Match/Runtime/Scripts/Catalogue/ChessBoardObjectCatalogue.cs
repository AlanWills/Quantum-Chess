using UnityEngine;
using Celeste.Objects;
using QC.Match.Objects;

namespace QC.Match.Catalogue
{
    [CreateAssetMenu(fileName = nameof(ChessBoardObjectCatalogue), menuName = "QC/Match/Chess Board Object Catalogue")]
    public class ChessBoardObjectCatalogue : ListScriptableObject<ChessBoardObject>, IAutomaticImportSettings
    {
        #region Properties and Fields

        public AutomaticImportBehaviour ImportBehaviour => importBehaviour;
        
        [SerializeField] private AutomaticImportBehaviour importBehaviour;
        
        #endregion
        
        public ChessBoardObject MustFindItemByGuid(int guid)
        {
            ChessBoardObject chessBoardObject = FindItem(x => x.Guid == guid);
            UnityEngine.Debug.Assert(chessBoardObject != null, $"Failed to find {nameof(ChessBoardObject)} wit guid '{guid}' in {name}.");
            return chessBoardObject;
        }
    }
}