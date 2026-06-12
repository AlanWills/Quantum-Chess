local SetActivePlayerCommand = { }

function SetActivePlayerCommand.to(index)
    MatchCommands.setActivePlayer(ChessBoardCommandExecutor, index)
end

return SetActivePlayerCommand