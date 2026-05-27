using System;
using UnityEngine;
using UnityEngine.Events;
using Celeste.Events;

namespace QC.Match.Events
{
    [Serializable]
    public class ChessBoardObjectRemovedUnityEvent : UnityEvent<ChessBoardObjectRemovedArgs> { }

    [Serializable]
    [CreateAssetMenu(fileName = nameof(ChessBoardObjectRemovedEvent), menuName = "QC/Events/Chess Board Object Removed Event")]
    public class ChessBoardObjectRemovedEvent : ParameterisedEvent<ChessBoardObjectRemovedArgs> { }
    
    [Serializable]
    public class GuaranteedChessBoardObjectRemovedEvent : GuaranteedParameterisedEvent<ChessBoardObjectRemovedEvent, ChessBoardObjectRemovedArgs> { }
}