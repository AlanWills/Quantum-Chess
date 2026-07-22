local EndTurnCommand = { }

function EndTurnCommand.new()
    MatchCommands.setActivePlayerIndex(BoardCommandExecutor, MatchState.nextPlayerIndex)
end

return EndTurnCommand