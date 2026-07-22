using Celeste.Components;
using Celeste.Objects;
using Celeste.Tools;
using QC.Match.Components;
using UnityEngine;

namespace QC.Match.Objects
{
    [CreateAssetMenu(fileName = nameof(BoardObject), menuName = "QC/Match/Objects/Board Object")]
    public class BoardObject : ComponentContainerUsingSubAssets<BoardObjectComponent>, IIntGuid
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