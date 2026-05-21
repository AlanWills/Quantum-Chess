using System;
using System.Collections.Generic;
using Celeste.Components;
using Lua;
using Lua.Unity;
using QC.Match.Catalogue;
using QC.Match.Components;
using QC.Match.Event;
using QC.Match.Persistence;
using UnityEngine;

namespace QC.Match.Objects
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

        private ChessBoardObjectInstance CreateInstance(ChessBoardObject chessBoardObject)
        {
            ChessBoardObjectInstance instance = new ChessBoardObjectInstance(chessBoardObject);
            EnsureInstanceComponentsExist(instance);

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

        private ChessBoardObjectInstance AddChessBoardObjectInstance(ChessBoardObjectInstance instance)
        {
            instance.ComponentDataChanged.AddListener(OnChessBoardObjectComponentDataChanged);
            objects.Add(instance);
            
            onChessBoardObjectAdded?.Invoke(new ChessBoardObjectAddedArgs(instance));

            return instance;
        }

        public ChessBoardObjectInstance AddChessBoardObject(ChessBoardObject chessBoardObject)
        {
            ChessBoardObjectInstance instance = new ChessBoardObjectInstance(chessBoardObject);
            EnsureInstanceComponentsExist(instance);
            
            return AddChessBoardObjectInstance(instance);
        }
        
        public ChessBoardObjectInstance AddChessBoardObject(ChessBoardObjectDTO chessBoardObjectDto)
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
        
        public ChessBoardObjectInstance GetChessBoardObjectAt(int x, int y)
        {
            return objects.Find(obj => 
                obj.TryFindComponent(out InterfaceHandle<IGrid2DTransform> grid2DTransform) && 
                grid2DTransform.iFace.GetPosition(grid2DTransform.instance) == new Vector2Int(x, y));
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

        [LuaMember("addChessPiece")]
        private void AddChessPiece(int guid, LuaTable config)
        {
            ChessBoardObject chessBoardObject = chessBoardObjectCatalogue.MustFindItemByGuid(guid);
            if (chessBoardObject == null)
            {
                return;
            }
            
            ChessBoardObjectInstance instance = AddChessBoardObject(chessBoardObject);
            if (instance == null)
            {
                return;
            }

            if (config == null)
            {
                return;
            }

            var chessPiece = instance.FindInterface<IChessPiece>();
            if (!chessPiece.IsValid)
            {
                UnityEngine.Debug.LogAssertion($"No {nameof(IChessPiece)} interface found on chess board object {guid}.  Did you mean to call 'addChessPiece'?");
                return;
            }

            // Set Colour
            {
                string colourString = config.GetString("colour");
                if (!string.IsNullOrEmpty(colourString) && Enum.TryParse(colourString, out ChessPieceColour colour))
                {
                    chessPiece.iFace.SetColour(chessPiece.instance, colour);
                }
            }
            
            // Set Type
            {
                if (config.TryGetValue("type", out LuaValue typeValue))
                {
                    if (typeValue.IsString())
                    {
                        string typeString = typeValue.As<string>();
                        if (Enum.TryParse(typeString, out ChessPieceType type))
                        {
                            chessPiece.iFace.SetType(chessPiece.instance, type);
                        }
                        
                    }
                    else if (typeValue.IsTable())
                    {
                        LuaTable typeTable = typeValue.As<LuaTable>();
                        ChessPieceType type = ChessPieceType.None;
                        
                        foreach (var typeString in typeTable)
                        {
                            if (Enum.TryParse(typeString.Value.As<string>(), out ChessPieceType subType))
                            {
                                type |= subType;
                            }
                        }
                        
                        chessPiece.iFace.SetType(chessPiece.instance, type);
                    }
                }
            }
            
            // Set Position
            {
                if (instance.TryFindComponent(out InterfaceHandle<IGrid2DTransform> gridTransform))
                {
                    int xPosition = config.GetInt("x");
                    int yPosition = config.GetInt("y");
                    gridTransform.iFace.SetPosition(gridTransform.instance, xPosition, yPosition);
                }
                else
                {
                    UnityEngine.Debug.LogAssertion($"No {nameof(IGrid2DTransform)} exists, this is a serious issue and should've been injected automatically, as it is a mandatory component.");
                }
            }
        }
        
        #endregion
    }
}