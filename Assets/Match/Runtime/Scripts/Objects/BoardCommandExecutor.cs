using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Lua;
using QC.Match.Commands;
using UnityEngine;

namespace QC.Match.Objects
{
    [LuaObject]
    [CreateAssetMenu(fileName = nameof(BoardCommandExecutor), menuName = "QC/Match/Command Executor")]
    public partial class BoardCommandExecutor : ScriptableObject, ICommandExecutor<BoardCommandArgs>
    {
        #region Properties and Fields

        public const string kLuaName = nameof(BoardCommandExecutor);
        
        [NonSerialized] private Channel<ICommand<BoardCommandArgs>> commands = Channel.CreateSingleConsumerUnbounded<ICommand<BoardCommandArgs>>();
        [NonSerialized] private CancellationTokenSource commandCancellationToken;
        
        #endregion

        public void BeginProcessing(BoardCommandArgs args)
        {
            commands = Channel.CreateSingleConsumerUnbounded<ICommand<BoardCommandArgs>>();
            commandCancellationToken = new CancellationTokenSource();
            
            ProcessCommandsAsync(args).Forget();
        }

        public void StopProcessing()
        {
            commands?.Writer.TryComplete();

            if (commandCancellationToken != null)
            {
                commandCancellationToken.Cancel();
                commandCancellationToken.Dispose();
                commandCancellationToken = null;
            }
        }

        public UniTask<CommandState> ExecuteCommandImmediately(ICommand<BoardCommandArgs> command, BoardCommandArgs args)
        {
            return command.Execute(args, commandCancellationToken.Token);
        }

        private async UniTask ProcessCommandsAsync(BoardCommandArgs args)
        {
            await UniTask.SwitchToMainThread();
            
            await foreach (var nextCommand in commands.Reader.ReadAllAsync(commandCancellationToken.Token))
            {
                await nextCommand.Execute(args, commandCancellationToken.Token);
                await UniTask.SwitchToMainThread();
            }
        }
        
        [LuaMember("addCommand")]
        public void QueueCommand(ICommand<BoardCommandArgs> command)
        {
            UnityEngine.Debug.Assert(commands != null, $"{nameof(commands)} is still null!  Did you forget to call {nameof(BeginProcessing)}?");
            UnityEngine.Debug.Assert(command != null, "Attempting to queue a null command!");
            commands.Writer.TryWrite(command);
        }
    }
}