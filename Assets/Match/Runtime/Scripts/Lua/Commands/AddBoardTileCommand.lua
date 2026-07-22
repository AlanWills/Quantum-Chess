local AddBoardTileCommand = { }

function AddBoardTileCommand.new(guid, config)
    MatchCommands.addBoardTile(BoardCommandExecutor, guid, config)
end

return AddBoardTileCommand