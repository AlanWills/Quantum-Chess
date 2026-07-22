local SelectCommand = { }

function SelectCommand.at(location)
    MatchCommands.selectAt(BoardCommandExecutor, location)
end

function SelectCommand.clear(location)
    MatchCommands.clearSelection(BoardCommandExecutor)
end

return SelectCommand