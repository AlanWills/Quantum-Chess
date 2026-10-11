using System;
using UnityEngine;
using UnityEngine.Events;
using Celeste.Events;

namespace QC.Match.Events
{
    [Serializable]
    public class BoardObjectSelectionChangedUnityEvent : UnityEvent<BoardObjectSelectionChangedArgs> { }

    [Serializable]
    [CreateAssetMenu(fileName = nameof(BoardObjectSelectionChangedEvent), menuName = "QC/Events/Board/Board Object Selection Changed Event")]
    public class BoardObjectSelectionChangedEvent : ParameterisedEvent<BoardObjectSelectionChangedArgs> { }
    
    [Serializable]
    public class GuaranteedBoardObjectSelectionChangedEvent : GuaranteedParameterisedEvent<BoardObjectSelectionChangedEvent, BoardObjectSelectionChangedArgs> { }
}