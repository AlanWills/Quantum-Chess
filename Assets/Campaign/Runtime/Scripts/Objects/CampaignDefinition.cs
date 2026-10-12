using System;
using System.Collections.Generic;
using Celeste.Objects;
using Celeste.Objects.Attributes.GUI;
using Celeste.Tools;
using Celeste.Tools.Attributes.GUI;
using UnityEngine;

namespace QC.Campaign.Objects
{
    [CreateAssetMenu(fileName = nameof(CampaignDefinition), menuName = "QC/Campaign/Campaign Definition")]
    public class CampaignDefinition : ScriptableObject, IGuid
    {
        #region Properties and Fields

        public string Guid
        {
            get => guid;
            set
            {
                if (string.CompareOrdinal(guid, value) == 0)
                {
                    guid = value;
                    EditorOnly.SetDirty(this);
                }
            }
        }

        public string DisplayName => displayName;

        public IReadOnlyList<string> Scenarios => scenarios;
        public IReadOnlyList<int> StartingUnits => startingUnits;

        [SerializeField, StringAsSystemGuid] private string guid;
        [SerializeField] private string displayName;
        [SerializeField, GuidPicker("MatchScenario")] private List<string> scenarios = new();
        [SerializeField, GuidPicker("BoardObject")] private List<int> startingUnits = new();

        #endregion
    }
}