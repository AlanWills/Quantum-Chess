local RemoveChessPieceCommand = { }

function RemoveChessPieceCommand.at(position)
    MatchCommands.removeChessPiece(ChessBoardCommandExecutor, position)
end

return RemoveChessPieceCommand