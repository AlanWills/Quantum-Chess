local EndTurnCommand = { }

function EndTurnCommand.new()
    MatchCommands.setActivePlayerIndex(ChessBoardCommandExecutor, MatchState.nextPlayerIndex)
end

return EndTurnCommand