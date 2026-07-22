local SetActivePlayerCommand = { }

function SetActivePlayerCommand.toIndex(index)
    MatchCommands.setActivePlayerIndex(BoardCommandExecutor, index)
end

function SetActivePlayerCommand.toColour(colour)
    local playerIndex = MatchState:findPlayerIndexOfColour(colour)
    MatchCommands.setActivePlayerIndex(BoardCommandExecutor, playerIndex)
end

return SetActivePlayerCommand