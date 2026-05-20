using System;
using CelesteEditor.Components;
using QC.Match.Components;
using QC.Match.Objects;
using UnityEditor;

namespace QCEditor.Match.Objects
{
    [CustomEditor(typeof(ChessBoardObject))]
    public class ChessBoardObjectEditor : ComponentContainerUsingSubAssetsEditor<ChessBoardObjectComponent>
    {
        protected override Type[] AllComponentTypes =>
            ChessBoardObjectEditorConstants.AllChessBoardObjectComponentTypes;
        protected override string[] AllComponentDisplayNames =>
            ChessBoardObjectEditorConstants.AllChessBoardObjectComponentDisplayNames;
    }
}