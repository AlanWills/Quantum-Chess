using Cysharp.Threading.Tasks;

namespace QC.Match.Commands
{
    public readonly struct MakePlayerActivePlayerCommand : ICommand<ChessBoardCommandArgs>
    {
        private readonly int PlayerIndex;

        public MakePlayerActivePlayerCommand(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }
        
        public UniTask<CommandState> Execute(ChessBoardCommandArgs args)
        {
            args.MatchState.MakePlayerActive(PlayerIndex);
            
            return UniTask.FromResult(CommandState.Completed);
        }
    }
}