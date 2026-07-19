using Cysharp.Threading.Tasks;
using QC.Match.Commands;
using QC.Match.Objects;

namespace QC.Match.Players
{
    public readonly struct ManualTurnStrategy : ITurnStrategy
    {
        private class State
        {
            public MatchState MatchState;
            public int TargetActions;
        }
        
        public UniTask OnTurnBegin(ITurnStrategy.Args args)
        {
            args.CommandExecutor.QueueCommand(new ToggleInputForChessPieceColourCommand(args.MatchState.ActiveColour, true));
            args.CommandExecutor.QueueCommand(new ToggleInputIfNotChessPieceColourCommand(args.MatchState.ActiveColour, false));
            
            return UniTask.CompletedTask;
        }

        public UniTask<ICommand<ChessBoardCommandArgs>> CalculateNextAction(ITurnStrategy.Args args)
        {
            return UniTask.FromResult<ICommand<ChessBoardCommandArgs>>(new EmptyCommand());
        }

        public async UniTask OnTurnEnd(ITurnStrategy.Args args)
        {
            State state = new State { MatchState = args.MatchState, TargetActions = 1 };
            await UniTask.WaitUntil(state, s => s.MatchState.GetNumActionsTakenByActivePlayer() >= s.TargetActions);

            args.CommandExecutor.QueueCommand(new ToggleInputForChessPieceColourCommand(args.MatchState.ActiveColour, false));
        }
    }
}