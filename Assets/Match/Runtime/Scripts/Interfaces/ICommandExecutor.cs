using Cysharp.Threading.Tasks;

namespace QC.Match
{
    public interface ICommandExecutor<TArgs>
    {
        void BeginProcessing(TArgs args);
        void QueueCommand(ICommand<TArgs> command);
        UniTask<CommandState> ExecuteCommandImmediately(ICommand<TArgs> command, TArgs args);
    }
}