local EndTurnCommand = { }

function EndTurnCommand.new()
    MatchCommands.endTurn(ChessBoardCommandExecutor)
end

return SetActiveColourCommand