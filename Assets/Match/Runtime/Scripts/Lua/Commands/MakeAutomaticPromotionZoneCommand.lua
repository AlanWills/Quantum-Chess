local MakeAutomaticPromotionZoneCommand = { }

function MakeAutomaticPromotionZoneCommand.new(position, promotions)
    MatchCommands.makeAutomaticPromotionZone(BoardCommandExecutor, position, promotions)
end

return MakeAutomaticPromotionZoneCommand