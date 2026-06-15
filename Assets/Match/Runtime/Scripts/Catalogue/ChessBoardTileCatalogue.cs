using UnityEngine;
using Celeste.Objects;
using QC.Match.Objects;

namespace QC.Match.Catalogue
{
    [CreateAssetMenu(fileName = nameof(ChessBoardTileCatalogue), menuName = "QC/Match/Tiles/Chess Board Tile Catalogue")]
    public class ChessBoardTileCatalogue : ListScriptableObject<ChessBoardTile>, IAutomaticImportSettings
    {
        #region Properties and Fields

        public AutomaticImportBehaviour ImportBehaviour => importBehaviour;

        [SerializeField] private AutomaticImportBehaviour importBehaviour =
            AutomaticImportBehaviour.ImportAssetsInCatalogueDirectoryAndSubDirectories;

        #endregion
        
        public ChessBoardTile MustFindItemByGuid(int guid)
        {
            ChessBoardTile chessBoardTile = FindItem(x => x.Guid == guid);
            UnityEngine.Debug.Assert(chessBoardTile != null, $"Failed to find {nameof(ChessBoardTile)} wit guid '{guid}' in {name}.");
            return chessBoardTile;
        }
    }
}