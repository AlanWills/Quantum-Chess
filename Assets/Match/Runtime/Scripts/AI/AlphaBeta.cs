using System;
using System.Collections.Generic;
using Celeste.Components;
using QC.Match.Commands;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.AI
{
    public static class AlphaBeta
    {
        private class Board : IChessBoardLookup
        {
           public ChessBoard chessBoard;
           public Dictionary<Vector2Int, Piece> pieces = new();

           public Board(ChessBoard chessBoard)
           {
              this.chessBoard = chessBoard;

              foreach (var chessBoardObject in chessBoard.Objects)
              {
                 if (chessBoardObject.TryFindComponent<IGrid2DTransform>(out var transform) &&
                     chessBoardObject.TryFindComponent<IChessPiece>(out var chessPiece) &&
                     chessBoardObject.TryFindComponent<IActionProvider>(out var actionProvider))
                 {
                    pieces[transform.iFace.GetPosition(transform.instance)] = new Piece
                    {
                       instance = chessBoardObject,
                       actionProvider = actionProvider,
                       chessPiece = chessPiece,
                    };
                 }
              }
           }

           private Board(Board board)
           {
              chessBoard = board.chessBoard;

              foreach (var piece in board.pieces)
              {
                 pieces[piece.Key] = piece.Value;
              }
           }
           
           public bool DoesChessBoardTileExist(Vector2Int boardPosition)
           {
              return chessBoard.DoesChessBoardTileExist(boardPosition);
           }

           public ChessBoardObjectInstance GetChessBoardObjectAt(Vector2Int boardPosition)
           {
              return pieces.TryGetValue(boardPosition, out var piece) ? piece.instance : null;
           }

           public void RemovePieceAt(Vector2Int position)
           {
              pieces.Remove(position);
           }

           public void ReplacePieceAt(Vector2Int position, Piece piece)
           {
              pieces[position] = piece;
           }

           public Board Clone()
           {
              return new Board(this);
           }
        }

        private struct Piece
        {
           public ChessBoardObjectInstance instance;
           public InterfaceHandle<IChessPiece> chessPiece;
           public InterfaceHandle<IActionProvider> actionProvider;
        }
        
        public static bool TryCalculate(
            List<ChessBoardObjectInstance> piecesToConsider,
            ITurnStrategy.Args args,
            out ICommand<ChessBoardCommandArgs> command)
        {
            
        }

        private static int AlphaBetaMax(
           Board board, 
           ChessPieceColour activeColour, 
           int alpha, 
           int beta, 
           int remainingDepth)
        {
           if (remainingDepth == 0)
           {
              return EvaluateBoard(board, activeColour);
           }
           
           int bestValue = int.MinValue;
           List<Vector2Int> validActions = new List<Vector2Int>();

           foreach (var piece in board.pieces)
           {
              validActions.Clear();
              
              var actionProvider = piece.Value.actionProvider;
              actionProvider.iFace.CalculateValidAttacks(actionProvider.instance, piece.Key, board, validActions);
              actionProvider.iFace.CalculateValidMoves(actionProvider.instance, piece.Key, board, validActions);

              foreach (var action in validActions)
              {
                 // Need to make a new representation of the board here taking into account the action (same for move or attack)
                 Board nextBoard = board.Clone();
                 nextBoard.RemovePieceAt(piece.Key);
                 nextBoard.ReplacePieceAt(action, piece.Value);
                 
                 int score = AlphaBetaMin(nextBoard, activeColour, alpha, beta, remainingDepth - 1);
                 if (score > bestValue)
                 {
                    bestValue = score;

                    if (score > alpha)
                    {
                       alpha = score;
                    }
                 }

                 if (score >= beta)
                 {
                    return score;
                 }
              }
           }
           
           return bestValue;
           
           /*
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

        private static int AlphaBetaMin(
           Board board, 
           ChessPieceColour activeColour, 
           int alpha, 
           int beta, 
           int remainingDepth)
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