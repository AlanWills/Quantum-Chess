local CheckPlayerWinConditionCommand = { }

function CheckPlayerWinConditionCommand.new(index)
    MatchCommands.checkWinConditionForPlayer(ChessBoardCommandExecutor, index)
end

return CheckPlayerWinConditionCommand