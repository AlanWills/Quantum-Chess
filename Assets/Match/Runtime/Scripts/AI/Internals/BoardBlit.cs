using System.Collections.Generic;
using Celeste.Core.Interfaces;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.AI.Internals
{
   public class BoardBlit : IBoardLookup
        {
           // TODO: Oooffff, why public?
           public readonly IList<TileBlit> board;
           private readonly int width;
           private readonly int xMin;
           private readonly int yMin;

           public BoardBlit(RectInt boardDimensions, IList<TileBlit> tileArray)
           {
              width = boardDimensions.size.x;
              xMin = boardDimensions.xMin;
              yMin = boardDimensions.yMin;
              board = tileArray;
           }

           public void SetTiles(
              IReadOnlyList<BoardTileInstance> tiles,
              AlphaBetaParameters alphaBetaParameters,
              IRNG rng)
           {
              foreach (var tile in tiles)
              {
                 Vector2Int tilePosition = tile.Position;
                 int index = ToIndex(tilePosition);
                 
                 UnityEngine.Debug.Assert(index < board.Count, $"Incorrectly calculated index ({index}) for position ({tilePosition}).");
                 int tileImportance = tile.FindInterface<ITileAIInfo>().GetImportance();
                 int tileImportanceOverride = alphaBetaParameters.GetImportanceOverrideForPosition(tilePosition, rng, tileImportance);
                 board[index] = new TileBlit(true, tileImportanceOverride);
              }
           }

           public void SetObjects(
              IReadOnlyList<BoardObjectInstance> objects,
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
                 TileBlit currentTile = board[boardIndex];
                 board[boardIndex] = currentTile.WithPiece(piece);
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
              
              TileBlit tileBlit = board[indexOfPosition];
              TileBlit tileBlitWithoutPiece = tileBlit.WithoutPiece();
              board[indexOfPosition] = tileBlitWithoutPiece;
              UnityEngine.Debug.Assert(tileBlit.Importance == tileBlitWithoutPiece.Importance);
              UnityEngine.Debug.Assert(tileBlit.Importance == board[indexOfPosition].Importance);
              
              TileBlit destinationTileBlit = board[indexOfDestination];
              TileBlit destinationTileBlitWithPiece = destinationTileBlit.WithPiece(moveToMake.PieceToMove);
              board[indexOfDestination] = destinationTileBlitWithPiece;
              UnityEngine.Debug.Assert(destinationTileBlit.Importance == destinationTileBlitWithPiece.Importance);
              UnityEngine.Debug.Assert(destinationTileBlit.Importance == board[indexOfDestination].Importance);
           }

           public void UndoMove(Move moveToUndo)
           {
              int indexOfPosition = ToIndex(moveToUndo.Position);
              int indexOfDestination = ToIndex(moveToUndo.Destination);
              
              TileBlit tileBlit = board[indexOfPosition];
              TileBlit tileBlitPiece = tileBlit.WithPiece(moveToUndo.PieceToMove);
              board[indexOfPosition] = tileBlitPiece;
              UnityEngine.Debug.Assert(tileBlit.Importance == tileBlitPiece.Importance);
              UnityEngine.Debug.Assert(tileBlit.Importance == board[indexOfPosition].Importance);
              
              TileBlit destinationTileBlit = board[indexOfDestination];
              TileBlit destinationTileBlitWithPiece = moveToUndo.PieceToCapture.IsValid ? 
                 destinationTileBlit.WithPiece(moveToUndo.PieceToCapture) : destinationTileBlit.WithoutPiece();
              board[indexOfDestination] = destinationTileBlitWithPiece;
              UnityEngine.Debug.Assert(destinationTileBlit.Importance == destinationTileBlitWithPiece.Importance);
              UnityEngine.Debug.Assert(destinationTileBlit.Importance == board[indexOfDestination].Importance);
           }

           public bool DoesBoardTileExist(Vector2Int boardPosition)
           {
              int index = ToIndex(boardPosition);
              return 0 <= index && index < board.Count && board[index].IsValid;
           }

           public BoardObjectInstance GetBoardObjectAt(Vector2Int boardPosition)
           {
              int index = ToIndex(boardPosition);
              return 0 <= index && index < board.Count ? board[index].Piece.Instance : null;
           }
        }
}