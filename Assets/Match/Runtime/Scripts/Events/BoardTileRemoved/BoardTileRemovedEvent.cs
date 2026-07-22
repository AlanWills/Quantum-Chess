using System;
using UnityEngine;
using UnityEngine.Events;
using Celeste.Events;

namespace QC.Match.Events
{
    [Serializable]
    public class BoardTileRemovedUnityEvent : UnityEvent<BoardTileRemovedArgs> { }

    [Serializable]
    [CreateAssetMenu(fileName = nameof(BoardTileRemovedEvent), menuName = "QC/Events/Board Tile Removed Event")]
    public class BoardTileRemovedEvent : ParameterisedEvent<BoardTileRemovedArgs> { }
    
    [Serializable]
    public class GuaranteedBoardTileRemovedEvent : GuaranteedParameterisedEvent<BoardTileRemovedEvent, BoardTileRemovedArgs> { }
}