local AddChessBoardTileCommand = { }

function AddChessBoardTileCommand.new(guid, config)
    MatchCommands.addChessBoardTile(ChessBoardCommandExecutor, guid, config)
end

return AddChessBoardTileCommand