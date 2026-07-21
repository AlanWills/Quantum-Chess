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
using QC.Match.Players;
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
                CreateAndAddChessBoardTile(tile);
            }
            
            foreach (var chessBoardObjectInstanceDTO in chessBoardDTO.objects)
            {
                CreateAndAddChessBoardObject(chessBoardObjectInstanceDTO);
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

        public ChessBoardTileInstance CreateChessBoardTile(int guid, Vector2Int position)
        {
            ChessBoardTile chessBoardTile = chessBoardTileCatalogue.MustFindItemByGuid(guid);
            if (chessBoardTile == null)
            {
                return null;
            }
            
            UnityEngine.Debug.Assert(!DoesChessBoardTileExist(position), $"Tile Instance at position {position} already exists!");
            return new ChessBoardTileInstance(chessBoardTile, position);
        }

        private void CreateAndAddChessBoardTile(ChessBoardTileDTO tileDTO)
        {
            ChessBoardTileInstance instance = CreateChessBoardTile(tileDTO.guid, tileDTO.position.v);
            if (instance == null)
            {
                UnityEngine.Debug.LogAssertion($"Failed to create and add chess board tile from dto with guid: {tileDTO.guid}.");
                return;
            }
            
            AddChessBoardTileInstance(instance);
        }

        public void AddChessBoardTileInstance(ChessBoardTileInstance tileInstance)
        {
            tiles.Add(tileInstance);
            tileInstance.ComponentDataChanged.AddListener(OnChessBoardTileComponentDataChanged);
            onChessBoardTileAdded?.Invoke(new ChessBoardTileAddedArgs(tileInstance));
            onChanged?.Invoke();
        }
        
        private void RemoveChessBoardTileAt(int indexOfTile)
        {
            if (0 > indexOfTile || indexOfTile >= tiles.Count)
            {
                return;
            }
            
            ChessBoardTileInstance tileInstance = tiles[indexOfTile];
            tiles.RemoveAt(indexOfTile);
            tileInstance.ComponentDataChanged.RemoveListener(OnChessBoardTileComponentDataChanged);
            onChessBoardTileRemoved?.Invoke(new ChessBoardTileRemovedArgs(tileInstance.Position));
            onChanged?.Invoke();
        }

        public bool DoesChessBoardTileExist(Vector2Int boardPosition)
        {
            foreach (var tile in tiles)
            {
                if (tile.Position == boardPosition)
                {
                    return true;
                }
            }

            return false;
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
        
        private void CreateAndAddChessBoardObject(ChessBoardObjectDTO chessBoardObjectDto)
        {
            ChessBoardObject chessBoardObject = chessBoardObjectCatalogue.MustFindItemByGuid(chessBoardObjectDto.guid);
            if (chessBoardObject == null)
            {
                return;
            }

            ChessBoardObjectInstance instance = new ChessBoardObjectInstance(chessBoardObject);
            EnsureInstanceComponentsExist(instance);
            instance.LoadComponents(chessBoardObjectDto.components.ToLookup());
            
            AddChessBoardObjectInstance(instance);
        }

        public void AddChessBoardObjectInstance(ChessBoardObjectInstance instance)
        {
            EnsureInstanceComponentsExist(instance);
            
            instance.ComponentDataChanged.AddListener(OnChessBoardObjectComponentDataChanged);
            objects.Add(instance);
            
            onChessBoardObjectAdded?.Invoke(new ChessBoardObjectAddedArgs(instance));
            onChanged?.Invoke();
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
            for (int i = 0; i < objects.Count; ++i)
            {
                var objectInstance = objects[i];

                if (objectInstance.TryFindComponent<IGrid2DTransform>(out var grid2DTransform) &&
                    grid2DTransform.GetPosition() == boardPosition)
                {
                    return objectInstance;
                }
            }

            return null;
        }

        public bool TryFindChessPiece(ChessPieceColour colour, ChessPieceType type, out ChessBoardObjectInstance instance)
        {
            foreach (var objectInstance in objects)
            {
                if (objectInstance.TryFindComponent<IChessPiece>(out var chessPiece) &&
                    chessPiece.GetPieceColour() == colour &&
                    chessPiece.GetPieceType().HasFlag(type))
                {
                    instance = objectInstance;
                    return true;
                }
            }

            instance = null;
            return false;
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

        public RectInt CalculateBoundingBox()
        {
            Vector2Int minCoords = new Vector2Int(int.MaxValue, int.MaxValue);
            Vector2Int maxCoords = new Vector2Int(int.MinValue, int.MinValue);

            for (int i = 0, n = tiles.Count; i < n; ++i)
            {
                Vector2Int tilePosition = tiles[i].Position;
                minCoords.x = Mathf.Min(tilePosition.x, minCoords.x);
                maxCoords.x = Mathf.Max(tilePosition.x, maxCoords.x);
                minCoords.y = Mathf.Min(tilePosition.y, minCoords.y);
                maxCoords.y = Mathf.Max(tilePosition.y, maxCoords.y);
            }

            return new RectInt(minCoords.x, minCoords.y, maxCoords.x + 1 - minCoords.x, maxCoords.y + 1 - minCoords.y);
        }

        public void UpdateBoardActionMaskForColour(BoardActionMask mask, ChessPieceColour colour)
        {
            List<Vector2Int> validAttacks = new List<Vector2Int>();

            mask.ResetAllTiles();
            
            foreach (ChessBoardObjectInstance instance in objects)
            {
                if (!instance.TryFindComponent<IChessPiece>(out var chessPiece))
                {
                    continue;
                }

                if (chessPiece.GetPieceColour() != colour)
                {
                    continue;
                }

                if (!instance.TryFindComponent<IActionProvider>(out var actionProvider) ||
                    !instance.TryFindComponent<IGrid2DTransform>(out var grid2DTransform))
                {
                    continue;
                }

                validAttacks.Clear();
                actionProvider.iFace.CalculateValidAttacks(actionProvider.instance, grid2DTransform.GetPosition(), this, validAttacks);

                foreach (Vector2Int validAttack in validAttacks)
                {
                    mask.SetTileAttackable(validAttack);
                }
            }
        }

        public ChessBoardDTO Serialize()
        {
            return new ChessBoardDTO(this);
        }
        
        #region Callback

        private void OnChessBoardObjectComponentDataChanged()
        {
            onChanged?.InvokeSilently();
        }
        
        private void OnChessBoardTileComponentDataChanged()
        {
            onChanged?.Invoke();
        }

        #endregion
        
        #region Lua
        
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

            for (int i = 0, n = objects.Count; i < n; ++i)
            {
                var objectInstance = objects[i];
                
                if (objectInstance.TryFindComponent<IChessPiece>(out var chessPiece) &&
                    (!colour.HasValue || colour.Value == chessPiece.GetPieceColour()) &&
                    (!type.HasValue || type.Value == chessPiece.GetPieceType()))
                {
                    return true;
                }
            }

            return false;
        }
        
        #endregion
    }
}