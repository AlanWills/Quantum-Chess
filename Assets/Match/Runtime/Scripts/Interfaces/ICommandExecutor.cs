namespace QC.Match
{
    public interface ICommandExecutor<TArgs>
    {
        void BeginProcessing(TArgs args);
        void QueueCommand(ICommand<TArgs> command);
    }
}