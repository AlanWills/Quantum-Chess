using System;
using CelesteEditor.Components;
using QC.Match.Components;
using QC.Match.Objects;
using UnityEditor;

namespace QCEditor.Match.Objects
{
    [CustomEditor(typeof(ChessBoardTile))]
    public class ChessBoardTileEditor : ComponentContainerUsingSubAssetsEditor<ChessBoardTileComponent>
    {
        protected override Type[] AllComponentTypes =>
            ChessBoardTileEditorConstants.AllChessBoardTileComponentTypes;
        protected override string[] AllComponentDisplayNames =>
            ChessBoardTileEditorConstants.AllChessBoardTileComponentDisplayNames;
    }
}