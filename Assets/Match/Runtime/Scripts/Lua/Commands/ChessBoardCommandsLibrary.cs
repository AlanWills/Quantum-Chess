using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Celeste.Lua;
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
            ChessBoard chessBoard = context.GetArgument<ChessBoard>(0);
            CommandExecutor commandExecutor = context.GetArgument<CommandExecutor>(1);
            int xStart = context.GetArgument<int>(2);
            int yStart = context.GetArgument<int>(3);
            int xEnd = context.GetArgument<int>(4);
            int yEnd = context.GetArgument<int>(5);

            ChessBoardObjectInstance chessBoardObjectInstance = chessBoard.GetChessBoardObjectAt(xStart, yStart);
            MoveChessBoardObjectCommand moveCommand = new MoveChessBoardObjectCommand(chessBoardObjectInstance, new Vector2Int(xEnd, yEnd));
            commandExecutor.QueueCommand(moveCommand);
            
            return new ValueTask<int>(context.Return());
        }
    }
}