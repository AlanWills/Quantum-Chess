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
            functions.Add(new LibraryFunction(kName, "move", Move));
        }
        
        private ValueTask<int> Move(
            LuaFunctionExecutionContext context,
            CancellationToken cancellationToken)
        {
            ChessBoardCommandExecutor chessBoardCommandExecutor = context.GetArgument<ChessBoardCommandExecutor>(0);
            LuaVector2Int start = context.GetArgument<LuaVector2Int>(1);
            LuaVector2Int end = context.GetArgument<LuaVector2Int>(2);
            
            MoveChessBoardObjectCommand moveCommand = new MoveChessBoardObjectCommand(start.v, end.v);
            chessBoardCommandExecutor.QueueCommand(moveCommand);
            
            return new ValueTask<int>(context.Return());
        }
    }
}