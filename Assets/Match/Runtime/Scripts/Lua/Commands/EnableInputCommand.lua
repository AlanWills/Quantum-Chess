local EnableInputCommand = { }

function EnableInputCommand.forColour(colour)
    MatchCommands.enableInputForColour(ChessBoardCommandExecutor, colour)
end

function EnableInputCommand.forWhite()
    EnableInputCommand.forColour("White")
end

function EnableInputCommand.forBlack()
    EnableInputCommand.forColour("Black")
end

return EnableInputCommand