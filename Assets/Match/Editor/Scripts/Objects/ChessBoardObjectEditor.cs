using System;
using CelesteEditor.Components;
using QC.Match.Components;
using QC.Match.Objects;
using UnityEditor;

namespace QCEditor.Match.Objects
{
    [CustomEditor(typeof(BoardObject))]
    public class ChessBoardObjectEditor : ComponentContainerUsingSubAssetsEditor<BoardObjectComponent>
    {
        protected override Type[] AllComponentTypes =>
            ChessBoardObjectEditorConstants.AllChessBoardObjectComponentTypes;
        protected override string[] AllComponentDisplayNames =>
            ChessBoardObjectEditorConstants.AllChessBoardObjectComponentDisplayNames;
    }
}