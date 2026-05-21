using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Managers
{
    public class CommandExecutorManager : MonoBehaviour
    {
        #region Properties and Fields
        
        [SerializeField] private CommandExecutor commandExecutor;
        
        #endregion
        
        #region Unity Methods

        private void Update()
        {
            commandExecutor.UpdateCommands(Time.deltaTime);
        }
        
        #endregion
    }
}