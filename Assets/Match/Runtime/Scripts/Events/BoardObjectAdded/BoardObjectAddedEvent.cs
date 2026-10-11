using System;
using UnityEngine;
using UnityEngine.Events;
using Celeste.Events;

namespace QC.Match.Events
{
    [Serializable]
    public class BoardObjectAddedUnityEvent : UnityEvent<BoardObjectAddedArgs> { }

    [Serializable]
    [CreateAssetMenu(fileName = nameof(BoardObjectAddedEvent), menuName = "QC/Events/Board/Board Object Added Event")]
    public class BoardObjectAddedEvent : ParameterisedEvent<BoardObjectAddedArgs> { }
    
    [Serializable]
    public class GuaranteedBoardObjectAddedEvent : GuaranteedParameterisedEvent<BoardObjectAddedEvent, BoardObjectAddedArgs> { }
}