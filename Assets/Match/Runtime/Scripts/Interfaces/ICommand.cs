namespace QC.Match
{
    public enum CommandState
    {
        Running,
        Completed,
        Abandoned
    }
    
    public interface ICommand<in TArgs>
    {
        CommandState Execute(TArgs args);
    }
}