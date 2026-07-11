using System.Collections.Generic;
using Celeste.Core.Interfaces;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.AI.Internals
{
   public class Board : IChessBoardLookup
        {
           // TODO: Oooffff, why public?
           public readonly Tile[] board;
           private readonly int width;
           private readonly int height;
           private readonly int xMin;
           private readonly int yMin;

           public Board(RectInt boardDimensions, Tile[] tileArray)
           {
              width = boardDimensions.size.x;
              height = boardDimensions.size.y;
              xMin = boardDimensions.xMin;
              yMin = boardDimensions.yMin;
              board = tileArray;
           }

           public void SetTiles(
              IReadOnlyList<ChessBoardTileInstance> tiles,
              AlphaBetaParameters alphaBetaParameters)
           {
              foreach (var tile in tiles)
              {
                 Vector2Int tilePosition = tile.Position;
                 int index = ToIndex(tilePosition);
                 
                 UnityEngine.Debug.Assert(index < board.Length, $"Incorrectly calculated index ({index}) for position ({tilePosition}).");
                 int tileImportance = tile.FindInterface<ITileAIInfo>().GetImportance();
                 int tileImportanceOverride = alphaBetaParameters.GetImportanceOverrideForPosition(tilePosition, tileImportance);
                 board[index] = new Tile(true, tileImportanceOverride);
              }
           }

           public void SetObjects(
              IReadOnlyList<ChessBoardObjectInstance> objects,
              AlphaBetaParameters parameters,
              IRNG rng)
           {
              foreach (var chessBoardObjectInstance in objects)
              {
                 var chessPiece = chessBoardObjectInstance.FindInterface<IChessPiece>();
                 var actionProvider = chessBoardObjectInstance.FindInterface<IActionProvider>();
                 var transform = chessBoardObjectInstance.FindInterface<IGrid2DTransform>();
                 Vector2Int piecePosition = transform.GetPosition();
                 int boardIndex = ToIndex(piecePosition);
                 
                 ChessPieceType pieceType = chessPiece.GetPieceType();
                 ChessPieceColour pieceColour = chessPiece.GetPieceColour();
                 int pieceValue = chessPiece.GetValue();
                 int pieceValueOverride = parameters.GetValueOverrideForPiece(pieceType, rng, pieceValue);
                 Piece piece = new Piece(chessBoardObjectInstance, pieceType, pieceColour, pieceValueOverride, actionProvider);
                 board[boardIndex] = board[boardIndex].WithPiece(piece);
              }
           }

           public int ToIndex(Vector2Int position)
           {
              int index = (position.x - xMin) + (position.y - yMin) * width;
              return index;
           }

           public Vector2Int ToVector2Int(int boardIndex)
           {
              return new Vector2Int((boardIndex % width) + xMin, (boardIndex / width) + yMin);
           }

           public Piece GetPieceAt(Vector2Int boardPosition)
           {
              int index = ToIndex(boardPosition);
              return board[index].Piece;
           }

           public void MakeMove(Move moveToMake)
           {
              int indexOfPosition = ToIndex(moveToMake.Position);
              int indexOfDestination = ToIndex(moveToMake.Destination);
              
              Tile tile = board[indexOfPosition];
              Tile tileWithoutPiece = tile.WithoutPiece();
              board[indexOfPosition] = tileWithoutPiece;
              UnityEngine.Debug.Assert(tile.Importance == tileWithoutPiece.Importance);
              UnityEngine.Debug.Assert(tile.Importance == board[indexOfPosition].Importance);
              
              Tile destinationTile = board[indexOfDestination];
              Tile destinationTileWithPiece = destinationTile.WithPiece(moveToMake.PieceToMove);
              board[indexOfDestination] = destinationTileWithPiece;
              UnityEngine.Debug.Assert(destinationTile.Importance == destinationTileWithPiece.Importance);
              UnityEngine.Debug.Assert(destinationTile.Importance == board[indexOfDestination].Importance);
           }

           public void UndoMove(Move moveToUndo)
           {
              int indexOfPosition = ToIndex(moveToUndo.Position);
              int indexOfDestination = ToIndex(moveToUndo.Destination);
              
              Tile tile = board[indexOfPosition];
              Tile tilePiece = tile.WithPiece(moveToUndo.PieceToMove);
              board[indexOfPosition] = tilePiece;
              UnityEngine.Debug.Assert(tile.Importance == tilePiece.Importance);
              UnityEngine.Debug.Assert(tile.Importance == board[indexOfPosition].Importance);
              
              Tile destinationTile = board[indexOfDestination];
              Tile destinationTileWithPiece = destinationTile.WithoutPiece();
              board[indexOfDestination] = destinationTileWithPiece;
              UnityEngine.Debug.Assert(destinationTile.Importance == destinationTileWithPiece.Importance);
              UnityEngine.Debug.Assert(destinationTile.Importance == board[indexOfDestination].Importance);
           }

           public bool DoesChessBoardTileExist(Vector2Int boardPosition)
           {
              int index = ToIndex(boardPosition);
              return 0 <= index && index < board.Length && board[index].IsValid;
           }

           public ChessBoardObjectInstance GetChessBoardObjectAt(Vector2Int boardPosition)
           {
              int index = ToIndex(boardPosition);
              return 0 <= index && index < board.Length ? board[index].Piece.Instance : null;
           }
        }
}