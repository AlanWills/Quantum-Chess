local AddChessPieceCommand = { }

function AddChessPieceCommand.new(guid, config)
    MatchCommands.addChessPiece(ChessBoardCommandExecutor, guid, config)
end

return AddChessPieceCommand