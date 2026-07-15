using System.Threading;
using Cysharp.Threading.Tasks;
using QC.Match.Objects;

namespace QC.Match.Commands
{
    public readonly struct WaitForActionsTakenByActivePlayerCommand : ICommand<ChessBoardCommandArgs>
    {
        private class State
        {
            public MatchState MatchState;
            public int TargetActions;
        }
        
        private readonly int targetActions;

        public WaitForActionsTakenByActivePlayerCommand(int targetActions)
        {
            this.targetActions = targetActions;
        }
        
        public async UniTask<CommandState> Execute(ChessBoardCommandArgs args, CancellationToken cancellationToken)
        {
            State state = new State { MatchState = args.MatchState, TargetActions = targetActions };
            await UniTask.WaitUntil(state, s => s.MatchState.GetNumActionsTakenByActivePlayer() >= s.TargetActions);

            return CommandState.Completed;
        }
    }
}