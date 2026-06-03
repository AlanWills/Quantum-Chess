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
            while (commands.Count > 0 && commands.Peek().Execute(args) != CommandState.Running)
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