using Celeste.Components;
using Celeste.Objects;
using Celeste.Tools;
using QC.Match.Components;
using UnityEngine;

namespace QC.Match.Objects
{
    [CreateAssetMenu(fileName = nameof(ChessBoardObject), menuName = "QC/Match/Objects/Chess Board Object")]
    public class ChessBoardObject : ComponentContainerUsingSubAssets<ChessBoardObjectComponent>, IIntGuid
    {
        #region Properties and Fields
        
        public int Guid
        {
            get => guid;
            set
            {
                if (guid != value)
                {
                    guid = value;
                    EditorOnly.SetDirty(this);
                }
            }
        }
        
        [SerializeField] private int guid;

        #endregion
    }
}