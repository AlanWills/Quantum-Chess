using System;
using Cysharp.Threading.Tasks;
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
        
        [NonSerialized] private Channel<ICommand<ChessBoardCommandArgs>> commands = Channel.CreateSingleConsumerUnbounded<ICommand<ChessBoardCommandArgs>>();
        
        #endregion

        public void BeginProcessing(ChessBoardCommandArgs args)
        {
            commands = Channel.CreateSingleConsumerUnbounded<ICommand<ChessBoardCommandArgs>>();
            ProcessCommandsAsync(args).Forget();
        }

        private async UniTask ProcessCommandsAsync(ChessBoardCommandArgs args)
        {
            while (await commands.Reader.WaitToReadAsync())
            {
                while (commands.Reader.TryRead(out var nextCommand))
                {
                    await nextCommand.Execute(args);
                }
            }
        }
        
        [LuaMember("addCommand")]
        public void QueueCommand(ICommand<ChessBoardCommandArgs> command)
        {
            UnityEngine.Debug.Assert(commands != null, $"{nameof(commands)} is still null!  Did you forget to call {nameof(BeginProcessing)}?");
            commands.Writer.TryWrite(command);
        }
    }
}