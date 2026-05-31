local MoveCommand = { }

function MoveCommand.fromTo(startPosition, endPosition)
    ChessBoardCommands.moveFromTo(ChessBoardCommandExecutor, startPosition, endPosition)
end

function MoveCommand.selectedTo(endPosition)
    ChessBoardCommands.moveSelectedTo(ChessBoardCommandExecutor, endPosition)
end

return MoveCommand