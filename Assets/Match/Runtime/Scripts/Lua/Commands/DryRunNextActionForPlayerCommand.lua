local DryRunNextActionForPlayerCommand = { }

function DryRunNextActionForPlayerCommand.new(index)
    MatchCommands.dryRunNextActionForPlayer(ChessBoardCommandExecutor, index)
end

return DryRunNextActionForPlayerCommand