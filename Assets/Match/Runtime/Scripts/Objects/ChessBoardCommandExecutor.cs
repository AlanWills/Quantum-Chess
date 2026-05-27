using System;
using System.Collections.Generic;
using Lua;
using QC.Match.Commands;
using UnityEngine;

namespace QC.Match.Objects
{
    [LuaObject]
    [CreateAssetMenu(fileName = nameof(ChessBoardCommandExecutor), menuName = "QC/Match/Command Executor")]
    public partial class ChessBoardCommandExecutor : ScriptableObject, ICommandExecutor<ChessBoardCommandArgs>
    {
        #region Properties and Fields

        public const string kLuaName = nameof(ChessBoardCommandExecutor);
        
        [NonSerialized] private readonly Queue<ICommand<ChessBoardCommandArgs>> commands = new();
        
        #endregion

        public void UpdateCommands(ChessBoardCommandArgs args)
        {
            if (commands.Count == 0)
            {
                return;
            }

            var currentCommand = commands.Peek();
            CommandState commandState = currentCommand.Execute(args);
            
            if (commandState == CommandState.Completed ||
                commandState == CommandState.Abandoned)
            {
                commands.Dequeue();
            }
        }
        
        [LuaMember("addCommand")]
        public void QueueCommand(ICommand<ChessBoardCommandArgs> command)
        {
            commands.Enqueue(command);
        }
    }
}