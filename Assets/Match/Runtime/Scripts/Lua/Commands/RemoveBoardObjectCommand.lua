local RemoveBoardObjectCommand = { }

function RemoveBoardObjectCommand.at(position)
    MatchCommands.removeBoardObjectAt(BoardCommandExecutor, position)
end

return RemoveBoardObjectCommand