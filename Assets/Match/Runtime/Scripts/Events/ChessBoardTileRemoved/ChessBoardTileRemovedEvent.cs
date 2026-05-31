using System;
using UnityEngine;
using UnityEngine.Events;
using Celeste.Events;

namespace QC.Match.Events
{
    [Serializable]
    public class ChessBoardTileRemovedUnityEvent : UnityEvent<ChessBoardTileRemovedArgs> { }

    [Serializable]
    [CreateAssetMenu(fileName = nameof(ChessBoardTileRemovedEvent), menuName = "QC/Events/Chess Board Tile Removed Event")]
    public class ChessBoardTileRemovedEvent : ParameterisedEvent<ChessBoardTileRemovedArgs> { }
    
    [Serializable]
    public class GuaranteedChessBoardTileRemovedEvent : GuaranteedParameterisedEvent<ChessBoardTileRemovedEvent, ChessBoardTileRemovedArgs> { }
}