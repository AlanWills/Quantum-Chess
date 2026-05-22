namespace QC.Match
{
    public interface ICommandExecutor<TArgs>
    {
        void QueueCommand(ICommand<TArgs> command);
        
        void UpdateCommands(TArgs args);
    }
}