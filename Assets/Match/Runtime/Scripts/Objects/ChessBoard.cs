using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Components;
using Celeste.DataStructures;
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

        public IReadOnlyList<ChessBoardTileInstance> Tiles => tiles;
        public IReadOnlyList<ChessBoardObjectInstance> Objects => objects;

        [SerializeField] private ChessBoardTileCatalogue chessBoardTileCatalogue;
        [SerializeField] private ChessBoardObjectCatalogue chessBoardObjectCatalogue;
        
        [Header("Events")] 
        [SerializeField] private ChessBoardTileAddedEvent onChessBoardTileAdded;
        [SerializeField] private ChessBoardTileRemovedEvent onChessBoardTileRemoved;
        [SerializeField] private ChessBoardObjectAddedEvent onChessBoardObjectAdded;
        [SerializeField] private ChessBoardObjectRemovedEvent onChessBoardObjectRemoved;
        [SerializeField] private Celeste.Events.Event onChanged;
        
        [NonSerialized] private readonly List<ChessBoardTileInstance> tiles = new();
        [NonSerialized] private readonly List<ChessBoardObjectInstance> objects = new();
        
        #endregion

        public void Initialize()
        {
            tiles.Clear();
            objects.Clear();
        }

        public void Initialize(ChessBoardDTO chessBoardDTO)
        {
            Initialize();

            foreach (ChessBoardTileDTO tile in chessBoardDTO.tiles)
            {
                CreateChessBoardTile(tile);
            }
            
            foreach (var chessBoardObjectInstanceDTO in chessBoardDTO.objects)
            {
                CreateChessBoardObject(chessBoardObjectInstanceDTO);
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

        private ChessBoardTileInstance CreateChessBoardTile(int guid, Vector2Int position)
        {
            ChessBoardTile chessBoardTile = chessBoardTileCatalogue.MustFindItemByGuid(guid);
            if (chessBoardTile == null)
            {
                return null;
            }
            
            UnityEngine.Debug.Assert(!DoesChessBoardTileExist(position), $"Tile Instance at position {position} already exists!");
            ChessBoardTileInstance tileInstance = new ChessBoardTileInstance(chessBoardTile, position);
            tiles.Add(tileInstance);
            onChessBoardTileAdded?.Invoke(new ChessBoardTileAddedArgs(tileInstance));
            onChanged?.Invoke();

            return tileInstance;
        }

        private ChessBoardTileInstance CreateChessBoardTile(ChessBoardTileDTO tileDTO)
        {
            return CreateChessBoardTile(tileDTO.guid, tileDTO.position.v);
        }
        
        private void RemoveChessBoardTileAt(int indexOfTile)
        {
            if (0 > indexOfTile || indexOfTile >= tiles.Count)
            {
                return;
            }
            
            ChessBoardTileInstance tileInstance = tiles[indexOfTile];
            tiles.RemoveAt(indexOfTile);
            onChessBoardTileRemoved?.Invoke(new ChessBoardTileRemovedArgs(tileInstance.Position));
            onChanged?.Invoke();
        }

        public bool DoesChessBoardTileExist(Vector2Int boardPosition)
        {
            return tiles.Exists(x => x.Position == boardPosition);
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
        
        private ChessBoardObjectInstance CreateChessBoardObject(ChessBoardObjectDTO chessBoardObjectDto)
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

        public ChessBoardObjectInstance AddChessBoardObjectInstance(ChessBoardObjectInstance instance)
        {
            EnsureInstanceComponentsExist(instance);
            
            instance.ComponentDataChanged.AddListener(OnChessBoardObjectComponentDataChanged);
            objects.Add(instance);
            
            onChessBoardObjectAdded?.Invoke(new ChessBoardObjectAddedArgs(instance));
            onChanged?.Invoke();
            
            return instance;
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
        
        public ChessBoardObjectInstance GetChessBoardObjectAt(Vector2Int boardPosition)
        {
            return objects.Find(obj => 
                obj.TryFindComponent(out InterfaceHandle<IGrid2DTransform> grid2DTransform) && 
                grid2DTransform.iFace.GetPosition(grid2DTransform.instance) == boardPosition);
        }

        public bool DoesChessPieceExist(ChessPieceColour colour, ChessPieceType type)
        {
            return objects.Exists(x => 
                x.TryFindComponent<IChessPiece>(out var chessPiece) &&
                colour == chessPiece.iFace.GetPieceColour(chessPiece.instance) &&
                chessPiece.iFace.GetPieceType(chessPiece.instance).HasFlag(type));
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

            for (int i = tiles.Count - 1; i >= 0; --i)
            {
                RemoveChessBoardTileAt(i);
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
        private void AddChessBoardTile(int guid, LuaVector2Int boardPosition)
        {
            CreateChessBoardTile(guid, boardPosition.v);
        }

        [LuaMember("isChessPieceAt")]
        private bool IsChessPieceAt(LuaVector2Int boardPosition)
        {
            return GetChessBoardObjectAt(boardPosition.v) != null;
        }
        
        [LuaMember("chessPieceExists")]
        private bool ChessPieceExists(LuaTable luaTable)
        {
            string colourString = luaTable.GetString("colour", string.Empty);
            string typeString = luaTable.GetString("type", string.Empty);
            ChessPieceColour? colour = null;
            ChessPieceType? type = null;
            
            if (!string.IsNullOrEmpty(colourString) && Enum.TryParse(colourString, out ChessPieceColour chessPieceColour))
            {
                colour = chessPieceColour;
            }

            if (!string.IsNullOrEmpty(typeString) && Enum.TryParse(typeString, out ChessPieceType chessPieceType))
            {
                type = chessPieceType;
            }

            return objects.Exists(x => 
                x.TryFindComponent<IChessPiece>(out var chessPiece) &&
                (!colour.HasValue || colour.Value == chessPiece.iFace.GetPieceColour(chessPiece.instance)) &&
                (!type.HasValue || type.Value == chessPiece.iFace.GetPieceType(chessPiece.instance)));
        }
        
        #endregion
    }
}