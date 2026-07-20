local ThinkForPlayerCommand = { }

function ThinkForPlayerCommand.new(index)
    MatchCommands.thinkForPlayer(ChessBoardCommandExecutor, index)
end

function ThinkForPlayerCommand.dryRun(index)
    MatchCommands.thinkForPlayer(ChessBoardCommandExecutor, index, true)
end

return ThinkForPlayerCommand