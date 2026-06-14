local SetActivePlayerCommand = { }

function SetActivePlayerCommand.toIndex(index)
    MatchCommands.setActivePlayerIndex(ChessBoardCommandExecutor, index)
end

function SetActivePlayerCommand.toColour(colour)
    local playerIndex = MatchState:findPlayerIndexOfColour(colour)
    MatchCommands.setActivePlayerIndex(ChessBoardCommandExecutor, playerIndex)
end

return SetActivePlayerCommand