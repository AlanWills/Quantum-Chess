using Celeste.RemoteConfig;
using QC.Match.AI;
using UnityEngine;

namespace QC.Match.Managers
{
    public class AIStrategyDefaultParametersManager : MonoBehaviour
    {
        #region Properties and Fields

        [SerializeField] private RemoteConfigRecord remoteConfigRecord;
        [SerializeField] private string remoteConfigKey = "AIStrategyDefaults";
        [SerializeField] private AIStrategyDefaultParameters aiStrategyDefaultParameters;

        #endregion
        
        #region Unity Methods

        private void OnEnable()
        {
            aiStrategyDefaultParameters.Initialize();
            
            SyncRemoteConfigValues();
        }
        
        #endregion

        private void SyncRemoteConfigValues()
        {
            if (remoteConfigRecord == null)
            {
                return;
            }

            remoteConfigRecord.OverrideObject(remoteConfigKey, aiStrategyDefaultParameters);
        }
        
        #region Callbacks

        public void OnRemoteConfigFetchedDataChanged()
        {
            SyncRemoteConfigValues();
        }
        
        #endregion
    }
}