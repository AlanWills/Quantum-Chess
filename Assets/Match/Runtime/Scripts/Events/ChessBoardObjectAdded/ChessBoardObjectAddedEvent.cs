using System;
using UnityEngine;
using UnityEngine.Events;
using Celeste.Events;

namespace QC.Match.Events
{
    [Serializable]
    public class ChessBoardObjectAddedUnityEvent : UnityEvent<ChessBoardObjectAddedArgs> { }

    [Serializable]
    [CreateAssetMenu(fileName = nameof(ChessBoardObjectAddedEvent), menuName = "QC/Events/Chess Board Object Added Event")]
    public class ChessBoardObjectAddedEvent : ParameterisedEvent<ChessBoardObjectAddedArgs> { }
    
    [Serializable]
    public class GuaranteedChessBoardObjectAddedEvent : GuaranteedParameterisedEvent<ChessBoardObjectAddedEvent, ChessBoardObjectAddedArgs> { }
}