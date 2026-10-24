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
      public readonly TileBlit[] board;
      private readonly int width;
      private readonly int height;
      private readonly int xMin;
      private readonly int yMin;
      private readonly int xMax;
      private readonly int yMax;

      public BoardBlit(RectInt boardDimensions, TileBlit[] tileArray)
      {
         width = boardDimensions.size.x;
         height = boardDimensions.size.y;
         xMin = boardDimensions.xMin;
         yMin = boardDimensions.yMin;
         xMax = xMin + width;
         yMax = yMin + height;
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

            UnityEngine.Debug.Assert(index < board.Length,
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
            ref TileBlit currentTile = ref board[boardIndex];
            UnityEngine.Debug.Assert(!currentTile.Piece.IsValid,
               $"Attempting to set a piece to {nameof(TileBlit)} at {piecePosition}, but a piece is already there!");
            currentTile.Piece = piece;
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
         return
#if ULTRA_DEBUG_CHECKS
            0 <= index && index < board.Length && 
#endif
            board[index].Piece;
      }

      public void MakeMove(in Move moveToMake)
      {
#if ULTRA_DEBUG_CHECKS
         UnityEngine.Debug.Assert(IsPositionInBoard(moveToMake.PositionX, moveToMake.PositionY), $"Attempting to make a move from position ({moveToMake.PositionX}, {moveToMake.PositionY}), but it is outside of board bounds.");
         UnityEngine.Debug.Assert(IsPositionInBoard(moveToMake.DestinationX, moveToMake.DestinationY), $"Attempting to make a move to destination ({moveToMake.DestinationX}, {moveToMake.DestinationY}), but it is outside of board bounds.");
#endif
         int indexOfPosition = ToIndex(moveToMake.PositionX, moveToMake.PositionY);
         int indexOfDestination = ToIndex(moveToMake.DestinationX, moveToMake.DestinationY);

         ref TileBlit tileBlit = ref board[indexOfPosition];
         tileBlit.Piece = default;
         
         ref TileBlit destinationTileBlit = ref board[indexOfDestination];
         destinationTileBlit.Piece = moveToMake.PieceToMove;
      }

      public void UndoMove(in Move moveToUndo)
      {
#if ULTRA_DEBUG_CHECKS
         UnityEngine.Debug.Assert(IsPositionInBoard(moveToUndo.PositionX, moveToUndo.PositionY), $"Attempting to undo a move from position ({moveToUndo.PositionX}, {moveToUndo.PositionY}), but it is outside of board bounds.");
         UnityEngine.Debug.Assert(IsPositionInBoard(moveToUndo.DestinationX, moveToUndo.DestinationY), $"Attempting to undo a move to destination ({moveToUndo.DestinationX}, {moveToUndo.DestinationY}), but it is outside of board bounds.");
#endif
         int indexOfPosition = ToIndex(moveToUndo.PositionX, moveToUndo.PositionY);
         int indexOfDestination = ToIndex(moveToUndo.DestinationX, moveToUndo.DestinationY);

         ref TileBlit tileBlit = ref board[indexOfPosition];
         tileBlit.Piece = moveToUndo.PieceToMove;

         ref TileBlit destinationTileBlit = ref board[indexOfDestination];
         destinationTileBlit.Piece = moveToUndo.PieceToCapture.IsValid ? moveToUndo.PieceToCapture : default;
      }

      public bool DoesBoardTileExist(int x, int y)
      {
         int index = ToIndex(x, y);
         return
#if ULTRA_DEBUG_CHECKS
            0 <= index && index < board.Length && 
#endif
            board[index].IsValid;
      }

      public BoardTileInstance GetBoardTileAt(int x, int y)
      {  
         int index = ToIndex(x, y);
         return 
#if ULTRA_DEBUG_CHECKS
            0 <= index && index < board.Length && 
#endif
            board[index].Instance;
      }

      public BoardObjectInstance GetBoardObjectAt(int x, int y)
      {
         int index = ToIndex(x, y);
         return
#if ULTRA_DEBUG_CHECKS
            0 <= index && index < board.Length && 
#endif
            board[index].Piece.Instance;
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