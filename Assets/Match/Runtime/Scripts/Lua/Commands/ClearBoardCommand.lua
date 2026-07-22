local ClearBoardCommand = { }

function ClearBoardCommand.new()
    MatchCommands.clearBoard(BoardCommandExecutor)
end

return ClearBoardCommand