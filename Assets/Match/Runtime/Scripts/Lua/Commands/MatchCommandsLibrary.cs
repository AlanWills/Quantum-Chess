using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Celeste.Lua;
using Celeste.Lua.Maths;
using Lua;
using Lua.Standard;
using QC.Match.Commands;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Lua
{
    public class MatchCommandsLibrary : ILuaLibrary
    {
        #region Properties and Fields

        public static readonly MatchCommandsLibrary Instance = new();
        private const string kName = nameof(MatchCommandsLibrary);
        
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
            functions.Add(new LibraryFunction(kName, "setActiveColour", SetActiveColour));
            functions.Add(new LibraryFunction(kName, "switchActiveColour", SwitchActiveColour));
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
            
            EnableInputForChessPieceColourCommand enableInputCommand = new EnableInputForChessPieceColourCommand(chessPieceColour);
            chessBoardCommandExecutor.QueueCommand(enableInputCommand);
            
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
        
        private ValueTask<int> SetActiveColour(
            LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            ChessBoardCommandExecutor chessBoardCommandExecutor = context.GetArgument<ChessBoardCommandExecutor>(0);
            string colourString = context.GetArgument<string>(1);

            if (Enum.TryParse(colourString, out ChessPieceColour chessPieceColour))
            {
                SetActiveChessPieceColourCommand setActiveColourCommand = new SetActiveChessPieceColourCommand(chessPieceColour);
                chessBoardCommandExecutor.QueueCommand(setActiveColourCommand);
            }
            else
            {
                UnityEngine.Debug.LogAssertion($"Could not parse colour string '{colourString}' to a vaild chess piece colour enum value.");
            }

            return new ValueTask<int>(context.Return());
        }
        
        private ValueTask<int> SwitchActiveColour(
            LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            ChessBoardCommandExecutor chessBoardCommandExecutor = context.GetArgument<ChessBoardCommandExecutor>(0);
            MatchState matchState = context.GetArgument<MatchState>(1);

            ChessPieceColour newColour = matchState.ActiveColour == ChessPieceColour.White
                ? ChessPieceColour.Black
                : ChessPieceColour.White;
            
            SetActiveChessPieceColourCommand setActiveColourCommand = new SetActiveChessPieceColourCommand(newColour);
            chessBoardCommandExecutor.QueueCommand(setActiveColourCommand);
            
            return new ValueTask<int>(context.Return());
        }
    }
}