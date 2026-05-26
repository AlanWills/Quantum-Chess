local SelectObjectCommand = { }

function SelectObjectCommand.new(location)
    ChessBoardCommands.selectAt(ChessBoardCommandExecutor, location)
end

return SelectObjectCommand