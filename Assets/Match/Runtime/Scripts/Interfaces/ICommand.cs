namespace QC.Match
{
    public enum CommandState
    {
        Running,
        Finished
    }
    
    public interface ICommand
    {
        CommandState Execute();
    }
}