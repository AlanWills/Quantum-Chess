using System;
using System.Collections.Generic;
using Lua;
using UnityEngine;

namespace QC.Match.Objects
{
    [LuaObject]
    [CreateAssetMenu(fileName = nameof(CommandExecutor), menuName = "QC/Match/Command Executor")]
    public partial class CommandExecutor : ScriptableObject, ICommandExecutor
    {
        #region Properties and Fields

        public const string kLuaName = nameof(CommandExecutor);
        
        [NonSerialized] private readonly Queue<ICommand> commands = new();
        
        #endregion

        public void UpdateCommands(float deltaTime)
        {
            if (commands.Count == 0)
            {
                return;
            }

            var currentCommand = commands.Peek();
            if (currentCommand.Execute() == CommandState.Finished)
            {
                commands.Dequeue();
            }
        }
        
        [LuaMember("addCommand")]
        public void QueueCommand(ICommand command)
        {
            commands.Enqueue(command);
        }
    }
}