using System;
using Lua;
using QC.Match.Events;
using QC.Match.Persistence;
using UnityEngine;

namespace QC.Match.Objects
{
    [LuaObject(kLuaName)]
    [CreateAssetMenu(fileName = nameof(MatchState), menuName = "QC/Match/Match State")]
    public partial class MatchState : ScriptableObject
    {
        #region Properties and Fields

        public const string kLuaName = nameof(MatchState);

        public ChessPieceColour ActiveColour
        {
            get => activeColour;
            set
            {
                if (activeColour != value)
                {
                    ChessPieceColour oldColour = activeColour;
                    activeColour = value;
                    activeColourChangedEvent.Invoke(new ActiveColourChangedArgs(oldColour, activeColour));
                    onChanged.Invoke();
                }
            }
        }

        [Header("Events")] 
        [SerializeField] private ActiveColourChangedEvent activeColourChangedEvent;
        [SerializeField] private Celeste.Events.Event onChanged;

        [NonSerialized] private ChessPieceColour activeColour;

        #endregion

        public void Initialize()
        {
            activeColour = ChessPieceColour.Unknown;
        }

        public void Initialize(MatchStateDTO dto)
        {
            activeColour = dto.activeColour;
        }

        public MatchStateDTO Serialize()
        {
            return new MatchStateDTO(this);
        }
    }
}