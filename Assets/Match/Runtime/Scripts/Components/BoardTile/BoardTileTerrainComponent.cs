using System;
using System.ComponentModel;
using Celeste.Components;
using UnityEngine;

namespace QC.Match.Components
{
    [DisplayName("Terrain")]
    public class BoardTileTerrainComponent : BoardTileComponent, ITileAIInfo
    {
        #region Save Data

        [Serializable]
        private class Save : ComponentData
        {
            public int importance;
        }
        
        #endregion
        
        #region Properties and Fields

        [SerializeField] private int startingImportance;

        #endregion

        public override ComponentData CreateData()
        {
            return new Save
            {
                importance = startingImportance
            };
        }

        public int GetImportance(Instance instance)
        {
            return (instance.data as Save).importance;
        }

        public void SetImportance(Instance instance, int importance)
        {
            Save data = instance.data as Save;
            if (data.importance != importance)
            {
                data.importance = importance;
                instance.events.ComponentDataChanged.Invoke();
            }
        }
    }
}