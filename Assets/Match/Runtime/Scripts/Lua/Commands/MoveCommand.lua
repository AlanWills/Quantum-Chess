local MoveCommand = { }

function MoveCommand.fromTo(startPosition, endPosition)
    MatchCommands.moveFromTo(BoardCommandExecutor, startPosition, endPosition)
end

function MoveCommand.selectedTo(endPosition)
    MatchCommands.moveSelectedTo(BoardCommandExecutor, endPosition)
end

return MoveCommand