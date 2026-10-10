using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Celeste.Lua;
using Celeste.Lua.Maths;
using Lua;
using Lua.Standard;
using Lua.Unity;
using QC.Match.Commands;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Lua
{
    public class MatchCommandsLibrary : ILuaLibrary
    {
        #region Properties and Fields

        public static readonly MatchCommandsLibrary Instance = new();
        private const string kName = "MatchCommands";
        
        public string Name => kName;
        public IReadOnlyList<LibraryFunction> Functions => functions;
        public LuaRuntime LuaRuntime { get; set; }
        
        private readonly List<LibraryFunction> functions = new();
        
        #endregion

        private MatchCommandsLibrary()
        {
            functions.Add(new LibraryFunction(kName, "moveFromTo", MoveFromTo));
            functions.Add(new LibraryFunction(kName, "moveSelectedTo", MoveSelectedTo));
            functions.Add(new LibraryFunction(kName, "selectAt", SelectAt));
            functions.Add(new LibraryFunction(kName, "clearSelection", ClearSelection));
            functions.Add(new LibraryFunction(kName, "enableInputForColour", EnableInputForColour));
            functions.Add(new LibraryFunction(kName, "attackFromTo", AttackFromTo));
            functions.Add(new LibraryFunction(kName, "attackWithSelectedTo", AttackWithSelectedTo));
            functions.Add(new LibraryFunction(kName, "setActivePlayerIndex", SetActivePlayerIndex));
            functions.Add(new LibraryFunction(kName, "addPiece", AddPiece));
            functions.Add(new LibraryFunction(kName, "addCustomisedPiece", AddCustomisedPiece));
            functions.Add(new LibraryFunction(kName, "removeBoardObjectAt", RemoveBoardObjectAt));
            functions.Add(new LibraryFunction(kName, "addBoardTile", AddBoardTile));
            functions.Add(new LibraryFunction(kName, "clearBoard", ClearBoard));
            functions.Add(new LibraryFunction(kName, "checkWinConditionForPlayer", CheckWinConditionForPlayer));
            functions.Add(new LibraryFunction(kName, "thinkForPlayer", ThinkForPlayer));
            functions.Add(new LibraryFunction(kName, "makeAutomaticPromotionZone", MakeAutomaticPromotionZone));
        }
        
        private ValueTask<int> MoveFromTo(
            LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            BoardCommandExecutor boardCommandExecutor = context.GetArgument<BoardCommandExecutor>(0);
            LuaVector2Int start = context.GetArgument<LuaVector2Int>(1);
            LuaVector2Int end = context.GetArgument<LuaVector2Int>(2);
            
            MoveBoardObjectFromToCommand moveFromToCommand = new MoveBoardObjectFromToCommand(start.v, end.v);
            boardCommandExecutor.QueueCommand(moveFromToCommand);
            
            return new ValueTask<int>(context.Return());
        }
        
        private ValueTask<int> MoveSelectedTo(
            LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            BoardCommandExecutor boardCommandExecutor = context.GetArgument<BoardCommandExecutor>(0);
            LuaVector2Int end = context.GetArgument<LuaVector2Int>(1);
            
            MoveSelectedBoardObjectToCommand moveSelectedToCommand = new MoveSelectedBoardObjectToCommand(end.v);
            boardCommandExecutor.QueueCommand(moveSelectedToCommand);
            
            return new ValueTask<int>(context.Return());
        }

        private ValueTask<int> SelectAt(
            LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            BoardCommandExecutor boardCommandExecutor = context.GetArgument<BoardCommandExecutor>(0);
            LuaVector2Int location = context.GetArgument<LuaVector2Int>(1);
            
            SelectBoardObjectAtCommand selectAtCommand = new SelectBoardObjectAtCommand(location.v);
            boardCommandExecutor.QueueCommand(selectAtCommand);
            
            return new ValueTask<int>(context.Return());
        }

        private ValueTask<int> ClearSelection(
            LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            BoardCommandExecutor boardCommandExecutor = context.GetArgument<BoardCommandExecutor>(0);
            ClearSelectedBoardObjectCommand clearSelectionCommand = new ClearSelectedBoardObjectCommand();
            boardCommandExecutor.QueueCommand(clearSelectionCommand);
            
            return new ValueTask<int>(context.Return());
        }

        private ValueTask<int> EnableInputForColour(
            LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            BoardCommandExecutor boardCommandExecutor = context.GetArgument<BoardCommandExecutor>(0);
            string colourString = context.GetArgument<string>(1);

            if (string.IsNullOrEmpty(colourString) ||
                !Enum.TryParse(colourString, out PieceColour chessPieceColour))
            {
                UnityEngine.Debug.LogAssertion($"Invalid colour: {colourString}.  Ignoring enableInputForColour command...");
                return new ValueTask<int>(context.Return()); 
            }
            
            ToggleInputForPieceColourCommand toggleInputCommand = new ToggleInputForPieceColourCommand(chessPieceColour, true);
            boardCommandExecutor.QueueCommand(toggleInputCommand);
            
            return new ValueTask<int>(context.Return());
        }
        
        private ValueTask<int> AttackFromTo(
            LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            BoardCommandExecutor boardCommandExecutor = context.GetArgument<BoardCommandExecutor>(0);
            LuaVector2Int start = context.GetArgument<LuaVector2Int>(1);
            LuaVector2Int end = context.GetArgument<LuaVector2Int>(2);
            
            AttackWithBoardObjectFromToCommand attackFromTo = new AttackWithBoardObjectFromToCommand(start.v, end.v);
            boardCommandExecutor.QueueCommand(attackFromTo);
            
            return new ValueTask<int>(context.Return());
        }
        
        private ValueTask<int> AttackWithSelectedTo( LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            BoardCommandExecutor boardCommandExecutor = context.GetArgument<BoardCommandExecutor>(0);
            LuaVector2Int end = context.GetArgument<LuaVector2Int>(1);
            AttackWithSelectedBoardObjectCommand attackWithSelectedTo = new AttackWithSelectedBoardObjectCommand(end.v);
            boardCommandExecutor.QueueCommand(attackWithSelectedTo);
            
            return new ValueTask<int>(context.Return());
        }
        
        private ValueTask<int> SetActivePlayerIndex(
            LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            BoardCommandExecutor boardCommandExecutor = context.GetArgument<BoardCommandExecutor>(0);
            int playerIndex = context.GetArgument<int>(1);
            SetActivePlayerCommand setActivePlayerCommand = new SetActivePlayerCommand(playerIndex);
            boardCommandExecutor.QueueCommand(setActivePlayerCommand);

            return new ValueTask<int>(context.Return());
        }
        
        private ValueTask<int> AddPiece(
            LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            BoardCommandExecutor boardCommandExecutor = context.GetArgument<BoardCommandExecutor>(0);
            int guid = context.GetArgument<int>(1);
            LuaTable config = context.GetArgument<LuaTable>(2);

            if (config == null)
            {
                return new ValueTask<int>(context.Return());
            }

            PieceVerticalDirection verticalDirection;
            Vector2Int position;
            bool interactable;
            PieceColour? pieceColour = null;
            PieceType? pieceType = null;
            int? value = null;
            
            // Vertical Direction
            {
                // This is technically OK to be missing, so we don't log assertions if there's a failure here
                config.TryGetEnum("verticalDirection", out verticalDirection);
            }
            
            // Position
            {
                if (!config.TryGetVector2Int("position", out position))
                {
                    position.x = config.GetInt("x");
                    position.y = config.GetInt("y");
                }
            }

            // Interactable
            {
                interactable = config.GetBool("interactable");
            }
            
            // Colour
            {
                if (config.TryGetEnum("colour", out PieceColour colour))
                {
                    pieceColour = colour;
                }
            }

            // Type
            {
                if (config.TryGetValue("type", out LuaValue typeValue))
                {
                    if (typeValue.IsString())
                    {
                        string typeString = typeValue.As<string>();
                        if (!Enum.TryParse(typeString, out PieceType type))
                        {
                            UnityEngine.Debug.LogAssertion($"Issue attempting to parse the string '{typeString}' as a valid type.");
                        }
                        else
                        {
                            pieceType = type;
                        }
                    }
                    else if (typeValue.IsTable())
                    {
                        LuaTable typeTable = typeValue.As<LuaTable>();
                        
                        foreach (var typeString in typeTable)
                        {
                            if (Enum.TryParse(typeString.Value.As<string>(), out PieceType subType))
                            {
                                pieceType = pieceType.HasValue ? pieceType | subType : subType;
                            }
                        }
                    }
                    else
                    {
                        UnityEngine.Debug.LogAssertion("Inputted value for piece type is not of the correct format.  Expect a string or array of strings.");
                    }
                }
            }

            // Value
            {
                config.TryGet("value", out value);
            }
            
            AddPieceCommand addPieceCommand = new AddPieceCommand(
                guid,
                verticalDirection,
                position,
                interactable,
                pieceColour,
                pieceType,
                value);
            boardCommandExecutor.QueueCommand(addPieceCommand);
            
            return new ValueTask<int>(context.Return());
        }
        
        private ValueTask<int> AddCustomisedPiece(
            LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            BoardCommandExecutor boardCommandExecutor = context.GetArgument<BoardCommandExecutor>(0);
            int guid = context.GetArgument<int>(1);
            LuaTable config = context.GetArgument<LuaTable>(2);

            if (config == null)
            {
                return new ValueTask<int>(context.Return());
            }

            PieceColour colour;
            PieceType type = PieceType.None;
            int value = 0;
            PieceVerticalDirection verticalDirection;
            Vector2Int position;
            bool interactable;
            
            // Colour
            {
                if (!config.TryGetEnum("colour", out colour))
                {
                    UnityEngine.Debug.LogAssertion("Issue attempting to parse the 'colour' field as a valid colour when adding chess piece.");
                }
            }

            // Type
            {
                if (!config.TryGetValue("type", out LuaValue typeValue))
                {
                    UnityEngine.Debug.LogAssertion("No type string provided when attempting to add chess piece.");
                }
                
                if (typeValue.IsString())
                {
                    string typeString = typeValue.As<string>();
                    if (!Enum.TryParse(typeString, out type))
                    {
                        UnityEngine.Debug.LogAssertion($"Issue attempting to parse the string '{typeString}' as a valid type.");
                    }
                }
                else if (typeValue.IsTable())
                {
                    LuaTable typeTable = typeValue.As<LuaTable>();
                        
                    foreach (var typeString in typeTable)
                    {
                        if (Enum.TryParse(typeString.Value.As<string>(), out PieceType subType))
                        {
                            type |= subType;
                        }
                    }
                }
                else
                {
                    UnityEngine.Debug.LogAssertion("Inputted value for piece type is not of the correct format.  Expect a string or array of strings.");
                }
            }

            // Value
            {
                config.TryGet("value", out value);
            }

            // Vertical Direction
            {
                // This is technically OK to be missing, so we don't log assertions if there's a failure here
                config.TryGetEnum("verticalDirection", out verticalDirection);
            }
            
            // Position
            {
                if (!config.TryGetVector2Int("position", out position))
                {
                    position.x = config.GetInt("x");
                    position.y = config.GetInt("y");
                }
            }

            // Interactable
            {
                interactable = config.GetBool("interactable");
            }
            
            AddPieceCommand addPieceCommand = new AddPieceCommand(
                guid,
                verticalDirection,
                position,
                interactable,
                colour,
                type,
                value);
            boardCommandExecutor.QueueCommand(addPieceCommand);
            
            return new ValueTask<int>(context.Return());
        }
        
        private ValueTask<int> RemoveBoardObjectAt(
            LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            BoardCommandExecutor boardCommandExecutor = context.GetArgument<BoardCommandExecutor>(0);
            LuaVector2Int position = context.GetArgument<LuaVector2Int>(1);
            
            RemoveBoardObjectAtCommand removeBoardObjectCommand = new RemoveBoardObjectAtCommand(position.v);
            boardCommandExecutor.QueueCommand(removeBoardObjectCommand);
            
            return new ValueTask<int>(context.Return());
        }

        private ValueTask<int> AddBoardTile(
            LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            BoardCommandExecutor boardCommandExecutor = context.GetArgument<BoardCommandExecutor>(0);
            int tileGuid = context.GetArgument<int>(1);
            LuaTable config = context.GetArgument<LuaTable>(2);

            Vector2Int position;
            int importance;

            // Position
            {
                if (!config.TryGetVector2Int("position", out position))
                {
                    position.x = config.GetInt("x");
                    position.y = config.GetInt("y");
                }
            }
            
            // Importance
            {
                config.TryGet("importance", out importance);
            }

            AddBoardTileCommand addBoardTileCommand = new AddBoardTileCommand(tileGuid, position, importance);
            boardCommandExecutor.QueueCommand(addBoardTileCommand);
            
            return new ValueTask<int>(context.Return());
        }
        
        private ValueTask<int> ClearBoard(
            LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            BoardCommandExecutor boardCommandExecutor = context.GetArgument<BoardCommandExecutor>(0);
            ClearBoardCommand clearBoardCommand = new ClearBoardCommand();
            boardCommandExecutor.QueueCommand(clearBoardCommand);
            
            return new ValueTask<int>(context.Return());
        }
        
        private ValueTask<int> CheckWinConditionForPlayer(
            LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            BoardCommandExecutor boardCommandExecutor = context.GetArgument<BoardCommandExecutor>(0);
            int playerIndex = context.GetArgument<int>(1);
            CheckPlayerWinConditionCommand checkPlayerWinCondition = new CheckPlayerWinConditionCommand(playerIndex);
            boardCommandExecutor.QueueCommand(checkPlayerWinCondition);
            
            return new ValueTask<int>(context.Return());
        }
        
        private ValueTask<int> ThinkForPlayer(
            LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            BoardCommandExecutor boardCommandExecutor = context.GetArgument<BoardCommandExecutor>(0);
            int playerIndex = context.GetArgument<int>(1);
            bool dryRun = context.HasArgument(2) && context.GetArgument<bool>(2);
            ThinkForPlayerCommand thinkForPlayer = new ThinkForPlayerCommand(playerIndex, dryRun);
            boardCommandExecutor.QueueCommand(thinkForPlayer);
            
            return new ValueTask<int>(context.Return());
        }
        
        private ValueTask<int> MakeAutomaticPromotionZone(
            LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            BoardCommandExecutor boardCommandExecutor = context.GetArgument<BoardCommandExecutor>(0);
            LuaVector2Int position = context.GetArgument<LuaVector2Int>(1);
            LuaTable promotionsTable = context.GetArgument<LuaTable>(2);
            List<Promotion> promotions = new List<Promotion>();

            foreach (var promotionsTableEntry in promotionsTable)
            {
                if (!promotionsTableEntry.Value.IsTable())
                {
                    UnityEngine.Debug.LogAssertion($"Expected table for promotion definition in {nameof(MakeAutomaticPromotionZone)} command.");
                    continue;
                }

                LuaTable promotionTable = promotionsTableEntry.Value.ReadTable();
                Promotion promotion = new();

                if (promotionTable.TryGetEnum("requiredPieceType", out PieceType requiredPieceType))
                {
                    promotion.requiredPieceType = requiredPieceType;
                }

                if (promotionTable.TryGetEnum("requiredPieceColour", out PieceColour pieceColour))
                {
                    promotion.requiredPieceColour = pieceColour;
                }

                if (promotionTable.TryGetEnum("promotedPieceType", out PieceType promotedPieceType))
                {
                    promotion.promotedPieceType = promotedPieceType;
                }
                
                promotions.Add(promotion);
            }
            
            boardCommandExecutor.QueueCommand(new MakeAutomaticPromotionZoneCommand(position.v, promotions));
            
            return new ValueTask<int>(context.Return());
        }
    }
}