local AttackCommand = { }

function AttackCommand.fromTo(startPosition, endPosition)
    ChessBoardCommands.attackFromTo(ChessBoardCommandExecutor, startPosition, endPosition)
end

function AttackCommand.selectedTo(endPosition)
    ChessBoardCommands.attackWithSelectedTo(ChessBoardCommandExecutor, endPosition)
end

return AttackCommand