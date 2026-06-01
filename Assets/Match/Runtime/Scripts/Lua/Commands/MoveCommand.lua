local MoveCommand = { }

function MoveCommand.fromTo(startPosition, endPosition)
    MatchCommands.moveFromTo(ChessBoardCommandExecutor, startPosition, endPosition)
end

function MoveCommand.selectedTo(endPosition)
    MatchCommands.moveSelectedTo(ChessBoardCommandExecutor, endPosition)
end

return MoveCommand