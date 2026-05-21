namespace QC.Match
{
    public interface ICommandExecutor
    {
        void QueueCommand(ICommand command);
        
        void UpdateCommands(float deltaTime);
    }
}