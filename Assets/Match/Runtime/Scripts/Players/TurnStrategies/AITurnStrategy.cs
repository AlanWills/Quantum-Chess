using System.Collections.Generic;
using System.Linq;
using Celeste.DataStructures;
using Cysharp.Threading.Tasks;
using Lua;
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
            
            // Can begin by just moving random pieces
            List<ChessBoardObjectInstance> opposingPieces = new List<ChessBoardObjectInstance>();
            List<Vector2Int> validAttacks = new();
            List<Vector2Int> validMoves = new();
            
            ChessPieceColour activeColour = args.MatchState.ActiveColour;

            // Get all active player pieces
            opposingPieces.AddRange(args.ChessBoard.Objects.Where(x =>
                x.TryFindComponent<IChessPiece>(out var chessPiece) &&
                chessPiece.iFace.GetPieceColour(chessPiece.instance) == activeColour));
            
            // Shuffle them up
            opposingPieces.Shuffle();

            for (int i = opposingPieces.Count - 1; i >= 0; --i)
            {
                ChessBoardObjectInstance chessPiece = opposingPieces[i];

                if (chessPiece.TryFindComponent<IGrid2DTransform>(out var transform) &&
                    chessPiece.TryFindComponent<IObjectActionProvider>(out var actionProvider))
                {
                    validAttacks.Clear();
                    validMoves.Clear();
                    
                    actionProvider.iFace.CalculateValidAttacks(
                        actionProvider.instance,
                        transform.iFace.GetPosition(transform.instance), 
                        args.ChessBoard, 
                        validAttacks);
                    
                    actionProvider.iFace.CalculateValidMoves(
                        actionProvider.instance,
                        transform.iFace.GetPosition(transform.instance), 
                        args.ChessBoard, 
                        validMoves);
                    
                    int totalActions = validAttacks.Count + validMoves.Count;
                    if (totalActions > 0)
                    {
                        int randomActionIndex = Random.Range(0, validAttacks.Count + validMoves.Count);

                        if (randomActionIndex < validAttacks.Count)
                        {
                            args.CommandExecutor.QueueCommand(new AttackWithChessBoardObjectFromToCommand(
                                transform.iFace.GetPosition(transform.instance),
                                validAttacks[randomActionIndex]));
                            return;
                        }
                        else
                        {
                            args.CommandExecutor.QueueCommand(new MoveChessBoardObjectToCommand(
                                chessPiece,
                                validMoves[randomActionIndex - validAttacks.Count]));
                            return;
                        }
                    }
                }
            }
            
            UnityEngine.Debug.LogAssertion($"Failed to calculate any valid move or attack for colour {args.MatchState.ActiveColour}.  Queueing up a change of colour...");
            args.CommandExecutor.QueueCommand(new SetActivePlayerCommand(args.MatchState.NextPlayerIndex));
        }

        public UniTask OnTurnEnd(ITurnStrategy.Args args)
        {
            return UniTask.CompletedTask;
        }
    }
}