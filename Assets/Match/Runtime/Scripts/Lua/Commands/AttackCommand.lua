local AttackCommand = { }

function AttackCommand.fromTo(startPosition, endPosition)
    MatchCommands.attackFromTo(BoardCommandExecutor, startPosition, endPosition)
end

function AttackCommand.selectedTo(endPosition)
    MatchCommands.attackWithSelectedTo(BoardCommandExecutor, endPosition)
end

return AttackCommand