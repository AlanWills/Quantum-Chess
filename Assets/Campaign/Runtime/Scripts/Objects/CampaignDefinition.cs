using System.Collections.Generic;
using Celeste.Objects;
using Celeste.Tools;
using UnityEngine;

namespace QC.Campaign.Objects
{
    [CreateAssetMenu(fileName = nameof(CampaignDefinition), menuName = "QC/Campaign/Campaign Definition")]
    public class CampaignDefinition : ScriptableObject, IIntGuid
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
        [SerializeField] private List<string> matchScenarios;

        #endregion
    }
}