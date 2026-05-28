using System;
using UnityEngine;
using UnityEngine.Events;
using Celeste.Events;

namespace QC.Match.Events
{
    [Serializable]
    public class ChessBoardTileAddedUnityEvent : UnityEvent<ChessBoardTileAddedArgs> { }

    [Serializable]
    [CreateAssetMenu(fileName = nameof(ChessBoardTileAddedEvent), menuName = "QC/Events/Chess Board Tile Added")]
    public class ChessBoardTileAddedEvent : ParameterisedEvent<ChessBoardTileAddedArgs> { }
    
    [Serializable]
    public class GuaranteedChessBoardPositionAddedEvent : GuaranteedParameterisedEvent<ChessBoardTileAddedEvent, ChessBoardTileAddedArgs> { }
}