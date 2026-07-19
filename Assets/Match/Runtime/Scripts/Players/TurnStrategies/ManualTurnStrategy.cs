using Cysharp.Threading.Tasks;
using QC.Match.Commands;
using QC.Match.Objects;

namespace QC.Match.Players
{
    public readonly struct ManualTurnStrategy : ITurnStrategy
    {
        private class State
        {
            public IPlayerInfo PlayerInfo;
            public int TargetActions;
        }
        
        public UniTask OnTurnBegin(ITurnStrategy.Args args)
        {
            args.CommandExecutor.QueueCommand(new ToggleInputForChessPieceColourCommand(args.PlayerColour, true));
            args.CommandExecutor.QueueCommand(new ToggleInputIfNotChessPieceColourCommand(args.PlayerColour, false));
            
            return UniTask.CompletedTask;
        }

        public UniTask<ICommand<ChessBoardCommandArgs>> CalculateNextAction(ITurnStrategy.Args args)
        {
            return UniTask.FromResult<ICommand<ChessBoardCommandArgs>>(new EmptyCommand());
        }

        public async UniTask OnTurnEnd(ITurnStrategy.Args args)
        {
            State state = new State { PlayerInfo = args.PlayerInfo, TargetActions = 1 };
            await UniTask.WaitUntil(state, s => s.PlayerInfo.GetNumActionsTakenByActivePlayer() >= s.TargetActions);

            args.CommandExecutor.QueueCommand(new ToggleInputForChessPieceColourCommand(args.PlayerColour, false));
        }
    }
}