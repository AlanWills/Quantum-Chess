using UnityEngine;
using Celeste.Objects;
using QC.Match.Objects;

namespace QC.Match.Catalogue
{
    [CreateAssetMenu(fileName = nameof(BoardTileCatalogue), menuName = "QC/Match/Tiles/Board Tile Catalogue")]
    public class BoardTileCatalogue : ListScriptableObject<BoardTile>, IAutomaticImportSettings
    {
        #region Properties and Fields

        public AutomaticImportBehaviour ImportBehaviour => importBehaviour;

        [SerializeField] private AutomaticImportBehaviour importBehaviour =
            AutomaticImportBehaviour.ImportAssetsInCatalogueDirectoryAndSubDirectories;

        #endregion
        
        public BoardTile MustFindItemByGuid(int guid)
        {
            BoardTile boardTile = FindItem(x => x.Guid == guid);
            UnityEngine.Debug.Assert(boardTile != null, $"Failed to find {nameof(BoardTile)} wit guid '{guid}' in {name}.");
            return boardTile;
        }
    }
}