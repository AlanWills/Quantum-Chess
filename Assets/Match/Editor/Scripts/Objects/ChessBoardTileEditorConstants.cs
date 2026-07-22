using System;
using CelesteEditor.Tools.Utils;
using QC.Match.Components;

namespace QCEditor.Match.Objects
{
    public static class ChessBoardTileEditorConstants
    {
        #region Properties and Fields

        public static readonly Type[] AllChessBoardTileComponentTypes;
        public static readonly string[] AllChessBoardTileComponentDisplayNames;

        #endregion

        static ChessBoardTileEditorConstants()
        {
            TypeExtensions.LoadTypes<BoardTileComponent>(ref AllChessBoardTileComponentTypes, ref AllChessBoardTileComponentDisplayNames);
        }
    }
}