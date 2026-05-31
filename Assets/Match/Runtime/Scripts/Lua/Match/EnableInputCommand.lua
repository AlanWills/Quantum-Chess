local MoveObjectCommand = { }

function MoveObjectCommand.fromTo(startPosition, endPosition)
    ChessBoardCommands.moveFromTo(ChessBoardCommandExecutor, startPosition, endPosition)
end

function MoveObjectCommand.selectedTo(endPosition)
    ChessBoardCommands.moveSelectedTo(ChessBoardCommandExecutor, endPosition)
end

return MoveObjectCommand