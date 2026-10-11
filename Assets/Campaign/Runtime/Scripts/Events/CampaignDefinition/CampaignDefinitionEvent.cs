using System;
using UnityEngine;
using UnityEngine.Events;
using Celeste.Events;
using QC.Campaign.Objects;

namespace QC.Campaign.Events
{
    [Serializable]
    public class CampaignDefinitionUnityEvent : UnityEvent<CampaignDefinition> { }

    [Serializable]
    [CreateAssetMenu(fileName = nameof(CampaignDefinitionEvent), menuName = "QC/Events/Campaign/Campaign Definition Event")]
    public class CampaignDefinitionEvent : ParameterisedEvent<CampaignDefinition> { }
    
    [Serializable]
    public class GuaranteedCampaignDefinitionEvent : GuaranteedParameterisedEvent<CampaignDefinitionEvent, CampaignDefinition> { }
}