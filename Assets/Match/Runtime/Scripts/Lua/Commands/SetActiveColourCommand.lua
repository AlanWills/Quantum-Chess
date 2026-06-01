local SetActiveColourCommand = { }

function SetActiveColourCommand.switch(colour)
    MatchCommands.switchActiveColour(ChessBoardCommandExecutor, MatchState)
end

function SetActiveColourCommand.toWhite()
    MatchCommands.setActiveColour(ChessBoardCommandExecutor, "White")
end

function SetActiveColourCommand.toBlack()
    MatchCommands.setActiveColour(ChessBoardCommandExecutor, "Black")
end

return SetActiveColourCommand