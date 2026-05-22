namespace QC.Match
{
    public enum CommandState
    {
        Running,
        Finished
    }
    
    public interface ICommand<in TArgs>
    {
        CommandState Execute(TArgs args);
    }
}