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
    public class ChessBoardCommandsLibrary : ILuaLibrary
    {
        #region Properties and Fields

        public static readonly ChessBoardCommandsLibrary Instance = new();
        private const string kName = "ChessBoardCommands";
        
        public string Name => kName;
        public IReadOnlyList<LibraryFunction> Functions => functions;
        public LuaRuntime LuaRuntime { get; set; }
        
        private readonly List<LibraryFunction> functions = new();
        
        #endregion

        private ChessBoardCommandsLibrary()
        {
            functions.Add(new LibraryFunction(kName, "moveFromTo", MoveFromTo));
            functions.Add(new LibraryFunction(kName, "moveSelectedTo", MoveSelectedTo));
            functions.Add(new LibraryFunction(kName, "selectAt", SelectAt));
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
            
            MoveSelectedChessBoardObjectFromToCommand moveSelectedToCommand = new MoveSelectedChessBoardObjectFromToCommand(end.v);
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
    }
}