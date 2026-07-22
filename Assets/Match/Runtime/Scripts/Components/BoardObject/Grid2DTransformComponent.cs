using System;
using System.ComponentModel;
using Celeste.Components;
using Celeste.Events;
using UnityEngine;

namespace QC.Match.Components
{
    [DisplayName("Grid 2D Transform")]
    public class Grid2DTransformComponent : BoardObjectComponent, IGrid2DTransform
    {
        #region Save Data

        [Serializable]
        private class SaveData : ComponentData
        {
            public Vector2Int currentPosition;
        }
        
        #endregion
        
        public override ComponentData CreateData()
        {
            return new SaveData();
        }

        public Vector2Int GetPosition(Instance instance)
        {
            return (instance.data as SaveData).currentPosition;
        }

        public void SetPosition(Instance instance, Vector2Int position)
        {
            SaveData saveData = instance.data as SaveData;
            if (saveData.currentPosition != position)
            {
                saveData.currentPosition = position;
                instance.events.ComponentDataChanged.Invoke();
            }
        }

        public void SetPosition(Instance instance, int x, int y)
        {
            SetPosition(instance, new Vector2Int(x, y));
        }
    }
}