local ThinkForPlayerCommand = { }

function ThinkForPlayerCommand.new(index)
    MatchCommands.thinkForPlayer(BoardCommandExecutor, index)
end

function ThinkForPlayerCommand.dryRun(index)
    MatchCommands.thinkForPlayer(BoardCommandExecutor, index, true)
end

return ThinkForPlayerCommand