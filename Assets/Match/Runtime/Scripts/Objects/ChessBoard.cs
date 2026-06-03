using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Components;
using Celeste.Lua.Maths;
using Celeste.Persistence.Wrappers;
using Lua;
using Lua.Unity;
using QC.Match.Catalogue;
using QC.Match.Components;
using QC.Match.Events;
using QC.Match.Persistence;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace QC.Match.Objects
{
    [LuaObject(kLuaName)]
    [CreateAssetMenu(fileName = nameof(ChessBoard), menuName = "QC/Match/Board State")]
    public partial class ChessBoard : ScriptableObject, IChessBoardLookup
    {
        #region Properties and Fields

        public const string kLuaName = nameof(ChessBoard);

        public IReadOnlyList<ChessBoardObjectInstance> Objects => objects;
        public IReadOnlyCollection<Vector2Int> BoardPositions => boardPositions;

        [SerializeField] private ChessBoardObjectCatalogue chessBoardObjectCatalogue;
        
        // Can change these to actual objects in a catalogue
        [SerializeField] private TileBase blackTile;
        [SerializeField] private TileBase whiteTile;

        [Header("Events")] 
        [SerializeField] private ChessBoardTileAddedEvent onChessBoardTileAdded;
        [SerializeField] private ChessBoardTileRemovedEvent onChessBoardTileRemoved;
        [SerializeField] private ChessBoardObjectAddedEvent onChessBoardObjectAdded;
        [SerializeField] private ChessBoardObjectRemovedEvent onChessBoardObjectRemoved;
        [SerializeField] private Celeste.Events.Event onChanged;
        
        [NonSerialized] private readonly List<ChessBoardObjectInstance> objects = new();
        [NonSerialized] private readonly HashSet<Vector2Int> boardPositions = new();
        
        #endregion

        public void Initialize(IReadOnlyList<Vector2Int> positions)
        {
            boardPositions.Clear();

            foreach (Vector2Int position in positions)
            {
                AddChessBoardTile(position);
            }
        }

        public void Initialize(ChessBoardDTO chessBoardDTO)
        {
            foreach (var chessBoardObjectInstanceDTO in chessBoardDTO.objects)
            {
                AddChessBoardObject(chessBoardObjectInstanceDTO);
            }

            boardPositions.Clear();

            foreach (SerializableVector2Int position in chessBoardDTO.boardPositions)
            {
                AddChessBoardTile(position.v);
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
        
        private void AddChessBoardTile(Vector2Int boardPosition)
        {
            // Possibly need to expand this to take sprite etc. have some more data
            bool isEvenYPosition = boardPosition.y % 2 == 0;
            TileBase tile = boardPosition.x % 2 == (isEvenYPosition ? 1 : 0) ? whiteTile : blackTile;
            
            boardPositions.Add(boardPosition);
            onChessBoardTileAdded?.Invoke(new ChessBoardTileAddedArgs(boardPosition, tile));
            onChanged?.Invoke();
        }
        
        private void RemoveChessBoardTile(Vector2Int boardPosition)
        {
            if (boardPositions.Remove(boardPosition))
            {
                boardPositions.Add(boardPosition);
                onChessBoardTileRemoved?.Invoke(new ChessBoardTileRemovedArgs(boardPosition));
                onChanged?.Invoke();
            }
        }

        public bool DoesChessBoardTileExist(Vector2Int boardPosition)
        {
            return boardPositions.Contains(boardPosition);
        }
        
        private void EnsureInstanceComponentsExist(ChessBoardObjectInstance instance)
        {
            if (!instance.HasComponent<IGrid2DTransform>())
            {
                // If we don't have a transform component assigned we inject one here, as all chess board objects must have one
                Grid2DTransformComponent grid2DTransform = CreateInstance<Grid2DTransformComponent>();
                instance.AddComponent(grid2DTransform);
            }
        }

        public ChessBoardObjectInstance AddChessBoardObjectInstance(ChessBoardObjectInstance instance)
        {
            EnsureInstanceComponentsExist(instance);
            
            instance.ComponentDataChanged.AddListener(OnChessBoardObjectComponentDataChanged);
            objects.Add(instance);
            
            onChessBoardObjectAdded?.Invoke(new ChessBoardObjectAddedArgs(instance));
            onChanged?.Invoke();
            
            return instance;
        }

        public ChessBoardObjectInstance CreateChessBoardObject(int guid)
        {
            ChessBoardObject chessBoardObject = chessBoardObjectCatalogue.MustFindItemByGuid(guid);
            if (chessBoardObject == null)
            {
                return null;
            }
            
            ChessBoardObjectInstance instance = new ChessBoardObjectInstance(chessBoardObject);
            EnsureInstanceComponentsExist(instance);
            
            return instance;
        }
        
        private ChessBoardObjectInstance AddChessBoardObject(ChessBoardObjectDTO chessBoardObjectDto)
        {
            ChessBoardObject chessBoardObject = chessBoardObjectCatalogue.MustFindItemByGuid(chessBoardObjectDto.guid);
            if (chessBoardObject == null)
            {
                return null;
            }

            ChessBoardObjectInstance instance = new ChessBoardObjectInstance(chessBoardObject);
            EnsureInstanceComponentsExist(instance);
            instance.LoadComponents(chessBoardObjectDto.components.ToLookup());
            
            return AddChessBoardObjectInstance(instance);
        }
        
        public ChessBoardObjectInstance GetChessBoardObjectAt(Vector2Int boardPosition)
        {
            return objects.Find(obj => 
                obj.TryFindComponent(out InterfaceHandle<IGrid2DTransform> grid2DTransform) && 
                grid2DTransform.iFace.GetPosition(grid2DTransform.instance) == boardPosition);
        }
        
        public ChessBoardObjectInstance GetChessBoardObjectAt(int x, int y)
        {
            return GetChessBoardObjectAt(new Vector2Int(x, y));
        }

        public bool RemoveChessBoardObject(ChessBoardObjectInstance instance)
        {
            bool result = objects.Remove(instance);
            if (result)
            {
                onChessBoardObjectRemoved?.Invoke(new ChessBoardObjectRemovedArgs(instance));
            }

            return result;
        }

        public void Clear()
        {
            for (int i = objects.Count - 1; i >= 0; --i)
            {
                RemoveChessBoardObject(objects[i]);
            }

            List<Vector2Int> boardPositionsClone = boardPositions.ToList();
            foreach (Vector2Int boardPosition in boardPositionsClone)
            {
                RemoveChessBoardTile(boardPosition);
            }
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
        
        [LuaMember("addChessBoardTile")]
        private void AddChessBoardTile(LuaVector2Int boardPosition)
        {
            AddChessBoardTile(boardPosition.v);
        }
        
        #endregion
    }
}