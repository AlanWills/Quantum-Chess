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
            functions.Add(new LibraryFunction(kName, "addChessPiece", AddChessPiece));
            functions.Add(new LibraryFunction(kName, "removeChessPieceAt", RemoveChessPieceAt));
            functions.Add(new LibraryFunction(kName, "clearChessBoard", ClearChessBoard));
        }
        
        private ValueTask<int> MoveFromTo(
            LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            ChessBoardCommandExecutor chessBoardCommandExecutor = context.GetArgument<ChessBoardCommandExecutor>(0);
            LuaVector2Int start = context.GetArgument<LuaVector2Int>(1);
            LuaVector2Int end = context.GetArgument<LuaVector2Int>(2);
            
            MoveChessBoardObjectFromToCommand moveFromToCommand = new MoveChessBoardObjectFromToCommand(start.v, end.v);
            chessBoardCommandExecutor.QueueCommand(moveFromToCommand);
            
            return new ValueTask<int>(context.Return());
        }
        
        private ValueTask<int> MoveSelectedTo(
            LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            ChessBoardCommandExecutor chessBoardCommandExecutor = context.GetArgument<ChessBoardCommandExecutor>(0);
            LuaVector2Int end = context.GetArgument<LuaVector2Int>(1);
            
            MoveSelectedChessBoardObjectToCommand moveSelectedToCommand = new MoveSelectedChessBoardObjectToCommand(end.v);
            chessBoardCommandExecutor.QueueCommand(moveSelectedToCommand);
            
            return new ValueTask<int>(context.Return());
        }

        private ValueTask<int> SelectAt(
            LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            ChessBoardCommandExecutor chessBoardCommandExecutor = context.GetArgument<ChessBoardCommandExecutor>(0);
            LuaVector2Int location = context.GetArgument<LuaVector2Int>(1);
            
            SelectChessBoardObjectAtCommand selectAtCommand = new SelectChessBoardObjectAtCommand(location.v);
            chessBoardCommandExecutor.QueueCommand(selectAtCommand);
            
            return new ValueTask<int>(context.Return());
        }

        private ValueTask<int> ClearSelection(
            LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            ChessBoardCommandExecutor chessBoardCommandExecutor = context.GetArgument<ChessBoardCommandExecutor>(0);
            ClearSelectedChessBoardObjectCommand clearSelectionCommand = new ClearSelectedChessBoardObjectCommand();
            chessBoardCommandExecutor.QueueCommand(clearSelectionCommand);
            
            return new ValueTask<int>(context.Return());
        }

        private ValueTask<int> EnableInputForColour(
            LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            ChessBoardCommandExecutor chessBoardCommandExecutor = context.GetArgument<ChessBoardCommandExecutor>(0);
            string colourString = context.GetArgument<string>(1);

            if (string.IsNullOrEmpty(colourString) ||
                !Enum.TryParse(colourString, out ChessPieceColour chessPieceColour))
            {
                UnityEngine.Debug.LogAssertion($"Invalid colour: {colourString}.  Ignoring enableInputForColour command...");
                return new ValueTask<int>(context.Return()); 
            }
            
            ToggleInputForChessPieceColourCommand toggleInputCommand = new ToggleInputForChessPieceColourCommand(chessPieceColour, true);
            chessBoardCommandExecutor.QueueCommand(toggleInputCommand);
            
            return new ValueTask<int>(context.Return());
        }
        
        private ValueTask<int> AttackFromTo(
            LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            ChessBoardCommandExecutor chessBoardCommandExecutor = context.GetArgument<ChessBoardCommandExecutor>(0);
            LuaVector2Int start = context.GetArgument<LuaVector2Int>(1);
            LuaVector2Int end = context.GetArgument<LuaVector2Int>(2);
            
            AttackWithChessBoardObjectFromToCommand attackFromTo = new AttackWithChessBoardObjectFromToCommand(start.v, end.v);
            chessBoardCommandExecutor.QueueCommand(attackFromTo);
            
            return new ValueTask<int>(context.Return());
        }
        
        private ValueTask<int> AttackWithSelectedTo( LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            ChessBoardCommandExecutor chessBoardCommandExecutor = context.GetArgument<ChessBoardCommandExecutor>(0);
            LuaVector2Int end = context.GetArgument<LuaVector2Int>(1);
            AttackWithSelectedChessBoardObjectCommand attackWithSelectedTo = new AttackWithSelectedChessBoardObjectCommand(end.v);
            chessBoardCommandExecutor.QueueCommand(attackWithSelectedTo);
            
            return new ValueTask<int>(context.Return());
        }
        
        private ValueTask<int> SetActivePlayerIndex(
            LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            ChessBoardCommandExecutor chessBoardCommandExecutor = context.GetArgument<ChessBoardCommandExecutor>(0);
            int playerIndex = context.GetArgument<int>(1);
            SetActivePlayerCommand setActivePlayerCommand = new SetActivePlayerCommand(playerIndex);
            chessBoardCommandExecutor.QueueCommand(setActivePlayerCommand);

            return new ValueTask<int>(context.Return());
        }
        
        private ValueTask<int> AddChessPiece(
            LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            ChessBoardCommandExecutor chessBoardCommandExecutor = context.GetArgument<ChessBoardCommandExecutor>(0);
            int guid = context.GetArgument<int>(1);
            LuaTable config = context.GetArgument<LuaTable>(2);

            if (config == null)
            {
                return new ValueTask<int>(context.Return());
            }

            ChessPieceColour colour;
            ChessPieceType type = ChessPieceType.None;
            ChessPieceVerticalDirection verticalDirection;
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
                        if (Enum.TryParse(typeString.Value.As<string>(), out ChessPieceType subType))
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
            
            AddChessPieceCommand addChessPieceCommand = new AddChessPieceCommand(
                guid,
                colour,
                type,
                verticalDirection,
                position,
                interactable);
            chessBoardCommandExecutor.QueueCommand(addChessPieceCommand);
            
            return new ValueTask<int>(context.Return());
        }
        
        private ValueTask<int> RemoveChessPieceAt(
            LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            ChessBoardCommandExecutor chessBoardCommandExecutor = context.GetArgument<ChessBoardCommandExecutor>(0);
            LuaVector2Int position = context.GetArgument<LuaVector2Int>(1);
            
            RemoveChessBoardObjectAtCommand removeChessBoardObjectCommand = new RemoveChessBoardObjectAtCommand(position.v);
            chessBoardCommandExecutor.QueueCommand(removeChessBoardObjectCommand);
            
            return new ValueTask<int>(context.Return());
        }
        
        private ValueTask<int> ClearChessBoard(
            LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            ChessBoardCommandExecutor chessBoardCommandExecutor = context.GetArgument<ChessBoardCommandExecutor>(0);
            ClearChessBoardCommand clearChessBoardCommand = new ClearChessBoardCommand();
            chessBoardCommandExecutor.QueueCommand(clearChessBoardCommand);
            
            return new ValueTask<int>(context.Return());
        }
    }
}