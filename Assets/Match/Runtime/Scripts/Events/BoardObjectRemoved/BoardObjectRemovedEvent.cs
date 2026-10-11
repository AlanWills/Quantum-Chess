using System;
using UnityEngine;
using UnityEngine.Events;
using Celeste.Events;

namespace QC.Match.Events
{
    [Serializable]
    public class BoardObjectRemovedUnityEvent : UnityEvent<BoardObjectRemovedArgs> { }

    [Serializable]
    [CreateAssetMenu(fileName = nameof(BoardObjectRemovedEvent), menuName = "QC/Events/Board/Board Object Removed Event")]
    public class BoardObjectRemovedEvent : ParameterisedEvent<BoardObjectRemovedArgs> { }
    
    [Serializable]
    public class GuaranteedBoardObjectRemovedEvent : GuaranteedParameterisedEvent<BoardObjectRemovedEvent, BoardObjectRemovedArgs> { }
}