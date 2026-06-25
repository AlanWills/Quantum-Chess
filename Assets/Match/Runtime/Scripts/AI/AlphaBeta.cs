using System.Collections.Generic;
using Celeste.Components;
using QC.Match.Commands;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.AI
{
    public static class AlphaBeta
    {
        private class Board
        {
           public Dictionary<Vector2Int, Piece> pieces = new();
        }

        private struct Piece
        {
           public ChessBoardObjectInstance instance;
           public InterfaceHandle<IGrid2DTransform> gridTransform;
           public InterfaceHandle<IChessPiece> chessPiece;
           public InterfaceHandle<IActionProvider> actionProvider;
        }
        
        public static bool TryCalculate(
            List<ChessBoardObjectInstance> piecesToConsider,
            ITurnStrategy.Args args,
            out ICommand<ChessBoardCommandArgs> command)
        {
            
        }

        private static int AlphaBetaMax(/*int alpha, int beta, int depthleft*/)
        {
           /*
            * if ( depthleft == 0 ) return evaluate();
              bestValue = -infinity;
              for ( all moves) {
                 score = alphaBetaMin( alpha, beta, depthleft - 1 );
                 if( score > bestValue )
                 {
                    bestValue = score;
                    if( score > alpha )
                       alpha = score; // alpha acts like max in MiniMax
                 }
                 if( score >= beta )
                    return score;   // fail soft beta-cutoff
              }
              return bestValue;
            */
        }

        private static int AlphaBetaMin(/*int alpha, int beta, int depthleft*/)
        {
           /*
            * if ( depthleft == 0 ) return -evaluate();
              bestValue = infinity;
              for ( all moves) {
                 score = alphaBetaMax( alpha, beta, depthleft - 1 );
                 if( score < bestValue)
                 {
                    bestValue = score;
                    if( score < beta )
                       beta = score; // beta acts like min in MiniMax
                 }
                 if( score <= alpha )
                    return score; // fail soft alpha-cutoff, break can also be used here
              }
              return bestValue;
            */
        }

        private static int EvaluateBoard(
           Board board,
           ChessPieceColour activeColour)
        {
           int total = 0;

           foreach (var piece in board.pieces)
           {
              var chessPieceComponent = piece.Value.chessPiece;
              int materialWorth = CalculateMaterialWorth(chessPieceComponent.iFace.GetPieceType(chessPieceComponent.instance));
              int colourModifier = chessPieceComponent.iFace.GetPieceColour(chessPieceComponent.instance) == activeColour ? 1 : -1;
              
              total += materialWorth * colourModifier;
           }

           return total;
        }

        private static int CalculateMaterialWorth(ChessPieceType chessPieceType)
        {
           switch (chessPieceType)
           {
              case ChessPieceType.King:
                 return 900;
              
              case ChessPieceType.Queen:
                 return 9;
              
              case ChessPieceType.Rook:
                 return 5;
              
              case ChessPieceType.Bishop:
                 return 3;
              
              case ChessPieceType.Knight:
                 return 3;
              
              case ChessPieceType.Pawn:
                 return 1;
              
              case ChessPieceType.None:
              default:
                 return 0;
           }
        }
    }
}