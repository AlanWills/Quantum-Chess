using System.Collections.Generic;
using System.Linq;
using Celeste.DataStructures;
using Celeste.Lua;
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

        public int Depth { get; private set; } = 1;

        public static AITurnStrategyData FromTable(LuaTable table)
        {
            AITurnStrategyData data = new();

            if (table.TryGet("depth", out int depth))
            {
                data.Depth = depth;
            }

            return data;
        }
    }
    
    public class AITurnStrategy : ITurnStrategy
    {
        private readonly AITurnStrategyData data;
        
        public AITurnStrategy(AITurnStrategyData data)
        {
            this.data = data;
        }

        public UniTask OnTurnBegin(ITurnStrategy.Args args)
        {
            List<ChessBoardObjectInstance> activePlayerPieces = new List<ChessBoardObjectInstance>();
            ChessPieceColour activeColour = args.MatchState.ActiveColour;
            activePlayerPieces.AddRange(args.ChessBoard.Objects.Where(x =>
                x.HasComponent<IActionProvider>() &&
                x.HasComponent<IGrid2DTransform>() &&
                x.TryFindComponent<IChessPiece>(out var chessPiece) &&
                chessPiece.iFace.GetPieceColour(chessPiece.instance) == activeColour));
            
            if (AlphaBeta.TryCalculate(args, activePlayerPieces, data.Depth, out var command))
            {
                args.CommandExecutor.QueueCommand(command);
            }
            else
            {
                UnityEngine.Debug.LogAssertion($"Failed to calculate any valid move or attack for colour {args.MatchState.ActiveColour}.  Queueing up a change of colour...");
                args.CommandExecutor.QueueCommand(new SetActivePlayerCommand(args.MatchState.NextPlayerIndex));
            }

            return UniTask.CompletedTask;
        }

        public UniTask OnTurnEnd(ITurnStrategy.Args args)
        {
            return UniTask.CompletedTask;
        }
    }
}