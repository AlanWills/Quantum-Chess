using System;
using System.Collections.Generic;
using Celeste.Components;
using Lua;
using QC.Match.Catalogue;
using QC.Match.Event;
using QC.Match.Objects;
using QC.Match.Persistence;
using UnityEngine;

namespace QC.Match.Record
{
    [LuaObject(kLuaName)]
    [CreateAssetMenu(fileName = nameof(ChessBoard), menuName = "QC/Match/Board State")]
    public partial class ChessBoard : ScriptableObject
    {
        #region Properties and Fields

        public const string kLuaName = nameof(ChessBoard);

        public IReadOnlyList<ChessBoardObjectInstance> Objects => objects;

        [SerializeField] private ChessBoardObjectCatalogue chessBoardObjectCatalogue;

        [Header("Events")] 
        [SerializeField] private ChessBoardObjectAddedEvent onChessBoardObjectAdded;
        [SerializeField] private Celeste.Events.Event onChanged;
        
        [NonSerialized] private readonly List<ChessBoardObjectInstance> objects = new();
        
        #endregion

        public void Initialize()
        {
        }

        public void Initialize(ChessBoardDTO chessBoardDTO)
        {
            foreach (var chessBoardObjectInstanceDTO in chessBoardDTO.objects)
            {
                AddChessBoardObject(chessBoardObjectInstanceDTO);
            }
        }

        public void Shutdown()
        {
            foreach (ChessBoardObjectInstance chessBoardObjectInstance in objects)
            {
                chessBoardObjectInstance.ComponentDataChanged.RemoveListener(OnChessBoardObjectComponentDataChanged);
                chessBoardObjectInstance.ShutdownComponents();
            }
        }

        public void AddChessBoardObject(ChessBoardObject chessBoardObject)
        {
            ChessBoardObjectInstance instance = new ChessBoardObjectInstance(chessBoardObject);
            AddChessBoardObjectInstance(instance);
        }
        
        public void AddChessBoardObject(ChessBoardObjectDTO chessBoardObjectDto)
        {
            ChessBoardObject chessBoardObject = chessBoardObjectCatalogue.MustFindItemByGuid(chessBoardObjectDto.guid);
            if (chessBoardObject == null)
            {
                return;
            }

            ChessBoardObjectInstance instance = new ChessBoardObjectInstance(chessBoardObject);
            instance.LoadComponents(chessBoardObjectDto.components.ToLookup());
            
            AddChessBoardObjectInstance(instance);
        }

        private void AddChessBoardObjectInstance(ChessBoardObjectInstance instance)
        {
            instance.ComponentDataChanged.AddListener(OnChessBoardObjectComponentDataChanged);
            objects.Add(instance);
            
            onChessBoardObjectAdded?.Invoke(new ChessBoardObjectAddedArgs(instance));
        }

        public ChessBoardDTO Serialize()
        {
            return new ChessBoardDTO(this);
        }
        
        #region Callback

        private void OnChessBoardObjectComponentDataChanged()
        {
            onChanged?.Invoke();
        }
        
        #endregion
        
        #region Lua

        [LuaMember("addChessBoardObject")]
        private void AddChessBoardObject(int guid)
        {
            ChessBoardObject chessBoardObject = chessBoardObjectCatalogue.MustFindItemByGuid(guid);
            if (chessBoardObject == null)
            {
                return;
            }
            
            AddChessBoardObject(chessBoardObject);
        }
        
        #endregion
    }
}