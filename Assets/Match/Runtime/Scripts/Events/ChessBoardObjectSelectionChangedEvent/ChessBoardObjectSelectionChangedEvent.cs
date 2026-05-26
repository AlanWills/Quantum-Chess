using System;
using UnityEngine;
using UnityEngine.Events;
using Celeste.Events;

namespace QC.Match.Events
{
    [Serializable]
    public class ChessBoardObjectSelectionChangedUnityEvent : UnityEvent<ChessBoardObjectSelectionChangedArgs> { }

    [Serializable]
    [CreateAssetMenu(fileName = nameof(ChessBoardObjectSelectionChangedEvent), menuName = "QC/Events/Chess Board Object Selection Changed Event")]
    public class ChessBoardObjectSelectionChangedEvent : ParameterisedEvent<ChessBoardObjectSelectionChangedArgs> { }
    
    [Serializable]
    public class GuaranteedChessBoardObjectSelectionChangedEvent : GuaranteedParameterisedEvent<ChessBoardObjectSelectionChangedEvent, ChessBoardObjectSelectionChangedArgs> { }
}