using System;
using UnityEngine;
using UnityEngine.Events;
using Celeste.Events;

namespace QC.Match.Events
{
    [Serializable]
    public class ActiveColourChangedUnityEvent : UnityEvent<ActiveColourChangedArgs> { }

    [Serializable]
    [CreateAssetMenu(fileName = nameof(ActiveColourChangedEvent), menuName = "QC/Events/Active Colour Changed Event")]
    public class ActiveColourChangedEvent : ParameterisedEvent<ActiveColourChangedArgs> { }
    
    [Serializable]
    public class GuaranteedActiveColourChangedEvent : GuaranteedParameterisedEvent<ActiveColourChangedEvent, ActiveColourChangedArgs> { }
}