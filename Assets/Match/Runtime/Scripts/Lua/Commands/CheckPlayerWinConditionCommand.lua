local CheckPlayerWinConditionCommand = { }

function CheckPlayerWinConditionCommand.new(index)
    MatchCommands.checkWinConditionForPlayer(BoardCommandExecutor, index)
end

return CheckPlayerWinConditionCommand