using System;
using CelesteEditor.Tools.Utils;
using QC.Match.Components;

namespace QCEditor.Match.Objects
{
    public static class ChessBoardObjectEditorConstants
    {
        #region Properties and Fields

        public static readonly Type[] AllChessBoardObjectComponentTypes;
        public static readonly string[] AllChessBoardObjectComponentDisplayNames;

        #endregion

        static ChessBoardObjectEditorConstants()
        {
            TypeExtensions.LoadTypes<BoardObjectComponent>(ref AllChessBoardObjectComponentTypes, ref AllChessBoardObjectComponentDisplayNames);
        }
    }
}