using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
      private readonly int height;
      private readonly int xMin;
      private readonly int yMin;
      private readonly int xMax;
      private readonly int yMax;

      public BoardBlit(RectInt boardDimensions, IList<TileBlit> tileArray)
      {
         width = boardDimensions.size.x;
         height = boardDimensions.size.y;
         xMin = boardDimensions.xMin;
         yMin = boardDimensions.yMin;
         xMax = xMin + width;
         yMax = yMax + height;
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
            int index = ToIndex(tilePosition.x, tilePosition.y);

            UnityEngine.Debug.Assert(index < board.Count,
               $"Incorrectly calculated index ({index}) for position ({tilePosition}).");
            int tileImportance = tile.FindInterface<ITileAIInfo>().GetImportance();
            int tileImportanceOverride =
               alphaBetaParameters.GetImportanceOverrideForPosition(tilePosition, rng, tileImportance);
            board[index] = new TileBlit(tile, true, tileImportanceOverride);
         }
      }

      public void SetObjects(
         IReadOnlyList<BoardObjectInstance> objects,
         AlphaBetaParameters parameters,
         IRNG rng)
      {
         foreach (var chessBoardObjectInstance in objects)
         {
            var chessPiece = chessBoardObjectInstance.FindInterface<IPiece>();
            var actionProvider = chessBoardObjectInstance.FindInterface<IActionProvider>();
            var transform = chessBoardObjectInstance.FindInterface<IGrid2DTransform>();
            Vector2Int piecePosition = transform.GetPosition();
            int boardIndex = ToIndex(piecePosition.x, piecePosition.y);

            PieceType pieceType = chessPiece.GetPieceType();
            PieceColour pieceColour = chessPiece.GetPieceColour();
            int pieceValue = chessPiece.GetValue();
            int pieceValueOverride = parameters.GetValueOverrideForPiece(pieceType, rng, pieceValue);
            Piece piece = new Piece(chessBoardObjectInstance, pieceType, pieceColour, pieceValueOverride,
               actionProvider);
            TileBlit currentTile = board[boardIndex];
            UnityEngine.Debug.Assert(!currentTile.Piece.IsValid,
               $"Attempting to set a piece to {nameof(TileBlit)} at {piecePosition}, but a piece is already there!");
            board[boardIndex] = currentTile.WithPiece(piece);
         }
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public int ToIndex(int x, int y)
      {
         int index = (x - xMin) + (y - yMin) * width;
         return index;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public int ToX(int boardIndex)
      {
         return (boardIndex % width) + xMin;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public int ToY(int boardIndex)
      {
         return (boardIndex / width) + yMin;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public Piece GetPieceAt(int x, int y)
      {
         int index = ToIndex(x, y);
         return 0 <= index && index < board.Count ? board[index].Piece : default;
      }

      public void MakeMove(Move moveToMake)
      {
#if ULTRA_DEBUG_CHECKS
         UnityEngine.Debug.Assert(IsPositionInBoard(moveToMake.PositionX, moveToMake.PositionY), $"Attempting to make a move from position ({moveToMake.PositionX}, {moveToMake.PositionY}), but it is outside of board bounds.");
         UnityEngine.Debug.Assert(IsPositionInBoard(moveToMake.DestinationX, moveToMake.DestinationY), $"Attempting to make a move to destination ({moveToMake.DestinationX}, {moveToMake.DestinationY}), but it is outside of board bounds.");
#endif
         int indexOfPosition = ToIndex(moveToMake.PositionX, moveToMake.PositionY);
         int indexOfDestination = ToIndex(moveToMake.DestinationX, moveToMake.DestinationY);

         TileBlit tileBlit = board[indexOfPosition];
         TileBlit tileBlitWithoutPiece = tileBlit.WithoutPiece();
         board[indexOfPosition] = tileBlitWithoutPiece;
#if ULTRA_DEBUG_CHECKS
         UnityEngine.Debug.Assert(tileBlit.Importance == tileBlitWithoutPiece.Importance);
         UnityEngine.Debug.Assert(tileBlit.Importance == board[indexOfPosition].Importance);
#endif

         TileBlit destinationTileBlit = board[indexOfDestination];
         TileBlit destinationTileBlitWithPiece = destinationTileBlit.WithPiece(moveToMake.PieceToMove);
         board[indexOfDestination] = destinationTileBlitWithPiece;
#if ULTRA_DEBUG_CHECKS
         UnityEngine.Debug.Assert(destinationTileBlit.Importance == destinationTileBlitWithPiece.Importance);
         UnityEngine.Debug.Assert(destinationTileBlit.Importance == board[indexOfDestination].Importance);
#endif
      }

      public void UndoMove(Move moveToUndo)
      {
#if ULTRA_DEBUG_CHECKS
         UnityEngine.Debug.Assert(IsPositionInBoard(moveToUndo.PositionX, moveToUndo.PositionY), $"Attempting to undo a move from position ({moveToUndo.PositionX}, {moveToUndo.PositionY}), but it is outside of board bounds.");
         UnityEngine.Debug.Assert(IsPositionInBoard(moveToUndo.DestinationX, moveToUndo.DestinationY), $"Attempting to undo a move to destination ({moveToUndo.DestinationX}, {moveToUndo.DestinationY}), but it is outside of board bounds.");
#endif
         int indexOfPosition = ToIndex(moveToUndo.PositionX, moveToUndo.PositionY);
         int indexOfDestination = ToIndex(moveToUndo.DestinationX, moveToUndo.DestinationY);

         TileBlit tileBlit = board[indexOfPosition];
         TileBlit tileBlitPiece = tileBlit.WithPiece(moveToUndo.PieceToMove);
         board[indexOfPosition] = tileBlitPiece;
#if ULTRA_DEBUG_CHECKS
         UnityEngine.Debug.Assert(tileBlit.Importance == tileBlitPiece.Importance);
         UnityEngine.Debug.Assert(tileBlit.Importance == board[indexOfPosition].Importance);
#endif

         TileBlit destinationTileBlit = board[indexOfDestination];
         TileBlit destinationTileBlitWithPiece = moveToUndo.PieceToCapture.IsValid
            ? destinationTileBlit.WithPiece(moveToUndo.PieceToCapture)
            : destinationTileBlit.WithoutPiece();
         board[indexOfDestination] = destinationTileBlitWithPiece;
#if ULTRA_DEBUG_CHECKS
         UnityEngine.Debug.Assert(destinationTileBlit.Importance == destinationTileBlitWithPiece.Importance);
         UnityEngine.Debug.Assert(destinationTileBlit.Importance == board[indexOfDestination].Importance);
#endif
      }

      public bool DoesBoardTileExist(int x, int y)
      {
         int index = ToIndex(x, y);
         return 0 <= index && index < board.Count && board[index].IsValid;
      }

      public BoardTileInstance GetBoardTileAt(int x, int y)
      {  
         int index = ToIndex(x, y);
         return 0 <= index && index < board.Count ? board[index].Instance : null;
      }

      public BoardObjectInstance GetBoardObjectAt(int x, int y)
      {
         int index = ToIndex(x, y);
         return 0 <= index && index < board.Count ? board[index].Piece.Instance : null;
      }

      public BoardObjectInstance GetBoardObjectAt_SkipBoundsCheck(int x, int y)
      {
         int index = ToIndex(x, y);
         return 0 <= index && index < board.Count ? board[index].Piece.Instance : null;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public bool IsPositionInBoard(int x, int y)
      {
         return x >= xMin &&
                y >= yMin &&
                x < xMax &&
                y < yMax;
      }
   }
}