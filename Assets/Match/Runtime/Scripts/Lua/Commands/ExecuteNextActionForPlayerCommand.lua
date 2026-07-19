local ExecuteNextActionForPlayerCommand = { }

function ExecuteNextActionForPlayerCommand.new(index)
    MatchCommands.executeNextActionForPlayer(ChessBoardCommandExecutor, index)
end

function ExecuteNextActionForPlayerCommand.dryRun(index)
    MatchCommands.executeNextActionForPlayer(ChessBoardCommandExecutor, index, true)
end

return ExecuteNextActionForPlayerCommand