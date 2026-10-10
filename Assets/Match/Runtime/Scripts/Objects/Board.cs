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
    [CreateAssetMenu(fileName = nameof(Board), menuName = "QC/Match/Board State")]
    public partial class Board : ScriptableObject, IBoardLookup
    {
        #region Properties and Fields

        public const string kLuaName = nameof(Board);

        public IReadOnlyList<BoardTileInstance> Tiles => tiles;
        public IReadOnlyList<BoardObjectInstance> Objects => objects;

        [SerializeField] private BoardTileCatalogue boardTileCatalogue;
        [SerializeField] private BoardObjectCatalogue boardObjectCatalogue;
        
        [Header("Events")] 
        [SerializeField] private BoardTileAddedEvent onBoardTileAdded;
        [SerializeField] private BoardTileRemovedEvent onBoardTileRemoved;
        [SerializeField] private BoardObjectAddedEvent onBoardObjectAdded;
        [SerializeField] private BoardObjectRemovedEvent onBoardObjectRemoved;
        [SerializeField] private Celeste.Events.Event onChanged;
        
        [NonSerialized] private readonly List<BoardTileInstance> tiles = new();
        [NonSerialized] private readonly List<BoardObjectInstance> objects = new();
        
        #endregion

        public void Initialize()
        {
            tiles.Clear();
            objects.Clear();
        }

        public void Initialize(BoardDTO boardDto)
        {
            Initialize();

            foreach (BoardTileDTO tile in boardDto.tiles)
            {
                CreateAndAddBoardTile(tile);
            }
            
            foreach (var boardObjectInstanceDTO in boardDto.objects)
            {
                CreateAndAddBoardObject(boardObjectInstanceDTO);
            }
        }

        public void Shutdown()
        {
            foreach (BoardObjectInstance boardObjectInstance in objects)
            {
                boardObjectInstance.ComponentDataChanged.RemoveListener(OnBoardObjectComponentDataChanged);
                boardObjectInstance.ShutdownComponents();
            }
        }

        public BoardTileInstance CreateBoardTile(int guid, Vector2Int position)
        {
            BoardTile boardTile = boardTileCatalogue.MustFindItemByGuid(guid);
            if (boardTile == null)
            {
                return null;
            }
            
            UnityEngine.Debug.Assert(!DoesBoardTileExist(position), $"Tile Instance at position {position} already exists!");
            return new BoardTileInstance(boardTile, position);
        }

        private void CreateAndAddBoardTile(BoardTileDTO tileDTO)
        {
            BoardTileInstance instance = CreateBoardTile(tileDTO.guid, tileDTO.position.v);
            if (instance == null)
            {
                UnityEngine.Debug.LogAssertion($"Failed to create and add chess board tile from dto with guid: {tileDTO.guid}.");
                return;
            }
            
            AddBoardTileInstance(instance);
        }

        public void AddBoardTileInstance(BoardTileInstance tileInstance)
        {
            tiles.Add(tileInstance);
            tileInstance.ComponentDataChanged.AddListener(OnBoardTileComponentDataChanged);
            onBoardTileAdded?.Invoke(new BoardTileAddedArgs(tileInstance));
            onChanged?.Invoke();
        }
        
        private void RemoveBoardTileAt(int indexOfTile)
        {
            if (0 > indexOfTile || indexOfTile >= tiles.Count)
            {
                return;
            }
            
            BoardTileInstance tileInstance = tiles[indexOfTile];
            tiles.RemoveAt(indexOfTile);
            tileInstance.ComponentDataChanged.RemoveListener(OnBoardTileComponentDataChanged);
            onBoardTileRemoved?.Invoke(new BoardTileRemovedArgs(tileInstance.Position));
            onChanged?.Invoke();
        }

        public bool DoesBoardTileExist(Vector2Int boardPosition)
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

        public BoardTileInstance GetBoardTileAt(Vector2Int boardPosition)
        {
            foreach (var tile in tiles)
            {
                if (tile.Position == boardPosition)
                {
                    return tile;
                }
            }

            return null;
        }

        public BoardObjectInstance CreateBoardObject(int guid)
        {
            BoardObject boardObject = boardObjectCatalogue.MustFindItemByGuid(guid);
            if (boardObject == null)
            {
                return null;
            }
            
            BoardObjectInstance instance = new BoardObjectInstance(boardObject);
            EnsureInstanceComponentsExist(instance);
            
            return instance;
        }
        
        private void CreateAndAddBoardObject(ChessBoardObjectDTO chessBoardObjectDto)
        {
            BoardObject boardObject = boardObjectCatalogue.MustFindItemByGuid(chessBoardObjectDto.guid);
            if (boardObject == null)
            {
                return;
            }

            BoardObjectInstance instance = new BoardObjectInstance(boardObject);
            EnsureInstanceComponentsExist(instance);
            instance.LoadComponents(chessBoardObjectDto.components.ToLookup());
            
            AddBoardObjectInstance(instance);
        }

        public void AddBoardObjectInstance(BoardObjectInstance instance)
        {
            EnsureInstanceComponentsExist(instance);
            
            instance.ComponentDataChanged.AddListener(OnBoardObjectComponentDataChanged);
            objects.Add(instance);
            
            onBoardObjectAdded?.Invoke(new BoardObjectAddedArgs(instance));
            onChanged?.Invoke();
        }
        
        private void EnsureInstanceComponentsExist(BoardObjectInstance instance)
        {
            if (!instance.HasInterface<IGrid2DTransform>())
            {
                // If we don't have a transform component assigned we inject one here, as all chess board objects must have one
                Grid2DTransformComponent grid2DTransform = CreateInstance<Grid2DTransformComponent>();
                instance.AddComponent(grid2DTransform);
            }
        }
        
        public BoardObjectInstance GetBoardObjectAt(Vector2Int boardPosition)
        {
            for (int i = 0; i < objects.Count; ++i)
            {
                var objectInstance = objects[i];

                if (objectInstance.TryFindInterface<IGrid2DTransform>(out var grid2DTransform) &&
                    grid2DTransform.GetPosition() == boardPosition)
                {
                    return objectInstance;
                }
            }

            return null;
        }

        public bool TryFindPiece(PieceColour colour, PieceType type, out BoardObjectInstance instance)
        {
            foreach (var objectInstance in objects)
            {
                if (objectInstance.TryFindInterface<IPiece>(out var chessPiece) &&
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

        public bool RemoveBoardObject(BoardObjectInstance instance)
        {
            bool result = objects.Remove(instance);
            if (result)
            {
                onBoardObjectRemoved?.Invoke(new BoardObjectRemovedArgs(instance));
            }

            return result;
        }
        
        public void Clear()
        {
            for (int i = objects.Count - 1; i >= 0; --i)
            {
                RemoveBoardObject(objects[i]);
            }

            for (int i = tiles.Count - 1; i >= 0; --i)
            {
                RemoveBoardTileAt(i);
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

        public BoardDTO Serialize()
        {
            return new BoardDTO(this);
        }
        
        #region Callback

        private void OnBoardObjectComponentDataChanged()
        {
            onChanged?.InvokeSilently();
        }
        
        private void OnBoardTileComponentDataChanged()
        {
            onChanged?.Invoke();
        }

        #endregion
        
        #region Lua
        
        [LuaMember("isChessPieceAt")]
        private bool IsChessPieceAt(LuaVector2Int boardPosition)
        {
            return GetBoardObjectAt(boardPosition.v) != null;
        }
        
        [LuaMember("chessPieceExists")]
        private bool ChessPieceExists(LuaTable luaTable)
        {
            string colourString = luaTable.GetString("colour", string.Empty);
            string typeString = luaTable.GetString("type", string.Empty);
            PieceColour? colour = null;
            PieceType? type = null;
            
            if (!string.IsNullOrEmpty(colourString) && Enum.TryParse(colourString, out PieceColour chessPieceColour))
            {
                colour = chessPieceColour;
            }

            if (!string.IsNullOrEmpty(typeString) && Enum.TryParse(typeString, out PieceType chessPieceType))
            {
                type = chessPieceType;
            }

            for (int i = 0, n = objects.Count; i < n; ++i)
            {
                var objectInstance = objects[i];
                
                if (objectInstance.TryFindInterface<IPiece>(out var chessPiece) &&
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