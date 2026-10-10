using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using QC.Match.Components;
using QC.Match.Objects;
using UnityEngine;

namespace QC.Match.Commands
{
    public readonly struct MakeAutomaticPromotionZoneCommand : ICommand<BoardCommandArgs>
    {
        private readonly Vector2Int boardPosition;
        private readonly List<Promotion> promotions;

        public MakeAutomaticPromotionZoneCommand(Vector2Int boardPosition, IReadOnlyList<Promotion> promotions)
        {
            this.boardPosition = boardPosition;
            this.promotions = new List<Promotion>(promotions);
        }
        
        public UniTask<CommandState> Execute(BoardCommandArgs args, CancellationToken cancellationToken)
        {
            if (!args.Board.DoesBoardTileExist(boardPosition))
            {
                return UniTask.FromResult(CommandState.Abandoned);
            }

            BoardTileInstance boardTileInstance = args.Board.GetBoardTileAt(boardPosition);
            if (boardTileInstance.TryFindComponent<AutomaticPromotionZoneComponent>(out var automaticPromotionZone))
            {
                automaticPromotionZone.component.SetPromotions(automaticPromotionZone.instance, promotions);
            }
            else
            {
                var automaticPromotionZoneComponent = args.ComponentCatalogue.CreateComponent<AutomaticPromotionZoneComponent>();
                automaticPromotionZoneComponent.component.SetPromotions(automaticPromotionZoneComponent.instance, promotions);
                boardTileInstance.AddComponent(automaticPromotionZoneComponent);
            }

            return UniTask.FromResult(CommandState.Completed);
        }
    }
}