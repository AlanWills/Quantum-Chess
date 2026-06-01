local SelectCommand = { }

function SelectCommand.at(location)
    MatchCommands.selectAt(ChessBoardCommandExecutor, location)
end

function SelectCommand.clear(location)
    MatchCommands.clearSelection(ChessBoardCommandExecutor)
end

return SelectCommand