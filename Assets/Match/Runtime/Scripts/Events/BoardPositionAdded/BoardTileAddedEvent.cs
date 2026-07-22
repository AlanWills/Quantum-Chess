using System;
using UnityEngine;
using UnityEngine.Events;
using Celeste.Events;

namespace QC.Match.Events
{
    [Serializable]
    public class BoardTileAddedUnityEvent : UnityEvent<BoardTileAddedArgs> { }

    [Serializable]
    [CreateAssetMenu(fileName = nameof(BoardTileAddedEvent), menuName = "QC/Events/Board Tile Added")]
    public class BoardTileAddedEvent : ParameterisedEvent<BoardTileAddedArgs> { }
    
    [Serializable]
    public class GuaranteedBoardPositionAddedEvent : GuaranteedParameterisedEvent<BoardTileAddedEvent, BoardTileAddedArgs> { }
}