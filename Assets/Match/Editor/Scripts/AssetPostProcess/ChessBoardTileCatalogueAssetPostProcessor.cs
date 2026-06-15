using CelesteEditor.Objects;
using QC.Match.Catalogue;
using QC.Match.Objects;

namespace QCEditor.Match.Persistence.AssetPostProcess
{
    public class ChessBoardTileCatalogueAssetPostProcessor : CataloguePostProcessor<ChessBoardTile, ChessBoardTileCatalogue>
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets, 
            string[] deletedAssets, 
            string[] movedAssets,
            string[] movedFromAssetPaths, 
            bool didDomainReload)
        {
            HandleOnPostprocessAllAssets(importedAssets, deletedAssets, movedAssets, movedFromAssetPaths, didDomainReload);
        }
    }
}