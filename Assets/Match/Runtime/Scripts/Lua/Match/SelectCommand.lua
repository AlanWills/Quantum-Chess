local SelectCommand = { }

function SelectCommand.new(location)
    ChessBoardCommands.selectAt(ChessBoardCommandExecutor, location)
end

return SelectCommand