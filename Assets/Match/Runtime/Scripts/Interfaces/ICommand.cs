using Cysharp.Threading.Tasks;

namespace QC.Match
{
    public enum CommandState
    {
        Completed,
        Abandoned
    }
    
    public interface ICommand<in TArgs>
    {
        UniTask<CommandState> Execute(TArgs args);
    }
}