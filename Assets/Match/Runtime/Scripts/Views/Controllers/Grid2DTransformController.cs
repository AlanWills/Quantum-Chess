using Celeste.Components;
using Celeste.Maths;
using QC.Match.Components;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Views
{
    public class Grid2DTransformController : InterfaceController<IGrid2DTransform, ChessBoardObjectComponent, ChessBoardObjectInstance>
    {
        #region Properties and Fields

        [SerializeField] private Transform gridTransform;
        
        #endregion
        
        protected override void DoHookup(IRuntimeAddedContext context)
        {
            SyncPosition();
        }

        protected override void DoShutdown()
        {
        }

        protected override void OnInterfaceDataChanged()
        {
            SyncPosition();
        }

        private void SyncPosition()
        {
            gridTransform.localPosition = Interface.iFace.GetPosition(Interface.instance).ToVector3();
        }
    }
}