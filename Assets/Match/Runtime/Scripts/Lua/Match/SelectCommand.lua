local SelectCommand = { }

function SelectCommand.at(location)
    ChessBoardCommands.selectAt(ChessBoardCommandExecutor, location)
end

function SelectCommand.clear(location)
    ChessBoardCommands.clearSelection(ChessBoardCommandExecutor)
end

return SelectCommand