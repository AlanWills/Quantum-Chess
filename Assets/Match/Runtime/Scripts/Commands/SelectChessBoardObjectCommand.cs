using QC.Match.Objects;

namespace QC.Match.Commands
{
    public readonly struct SelectChessBoardObjectCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly ChessBoardObjectInstance chessBoardObjectInstance;
        
        public SelectChessBoardObjectCommand(ChessBoardObjectInstance instance)
        {
            chessBoardObjectInstance = instance;
        }
        
        public CommandState Execute(ChessBoardCommandArgs args)
        {
            if (chessBoardObjectInstance == null)
            {
                UnityEngine.Debug.LogAssertion($"Attempting to select a {nameof(ChessBoardObjectInstance)} that is null.");
                return CommandState.Finished;
            }
            
            args.Selection.CurrentSelectedObject = chessBoardObjectInstance;
            return CommandState.Finished;
        }
    }
}