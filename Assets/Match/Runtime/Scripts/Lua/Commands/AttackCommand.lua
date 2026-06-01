local AttackCommand = { }

function AttackCommand.fromTo(startPosition, endPosition)
    MatchCommands.attackFromTo(ChessBoardCommandExecutor, startPosition, endPosition)
end

function AttackCommand.selectedTo(endPosition)
    MatchCommands.attackWithSelectedTo(ChessBoardCommandExecutor, endPosition)
end

return AttackCommand