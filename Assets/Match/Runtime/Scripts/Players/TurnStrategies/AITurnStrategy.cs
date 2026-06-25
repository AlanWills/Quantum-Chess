using System.Collections.Generic;
using System.Linq;
using Celeste.DataStructures;
using Cysharp.Threading.Tasks;
using Lua;
using QC.Match.AI;
using QC.Match.Commands;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Players
{
    public class AITurnStrategyData
    {
        public static readonly AITurnStrategyData Default = new();

        public static AITurnStrategyData FromTable(LuaTable table)
        {
            return new();
        }
    }
    
    public class AITurnStrategy : ITurnStrategy
    {
        private readonly AITurnStrategyData data;
        
        public AITurnStrategy(AITurnStrategyData data)
        {
            this.data = data;
        }

        public async UniTask OnTurnBegin(ITurnStrategy.Args args)
        {
            await UniTask.Delay(3000);
            
            List<ChessBoardObjectInstance> activePlayerPieces = new List<ChessBoardObjectInstance>();
            ChessPieceColour activeColour = args.MatchState.ActiveColour;

            // Get all active player pieces
            activePlayerPieces.AddRange(args.ChessBoard.Objects.Where(x =>
                x.TryFindComponent<IChessPiece>(out var chessPiece) &&
                chessPiece.iFace.GetPieceColour(chessPiece.instance) == activeColour));

            if (RandomAction.TryCalculate(activePlayerPieces, args, out var command))
            {
                args.CommandExecutor.QueueCommand(command);
            }
            else
            {
                UnityEngine.Debug.LogAssertion($"Failed to calculate any valid move or attack for colour {args.MatchState.ActiveColour}.  Queueing up a change of colour...");
                args.CommandExecutor.QueueCommand(new SetActivePlayerCommand(args.MatchState.NextPlayerIndex));
            }
        }

        public UniTask OnTurnEnd(ITurnStrategy.Args args)
        {
            return UniTask.CompletedTask;
        }
    }
}