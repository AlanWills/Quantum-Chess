local RemoveChessPieceCommand = { }

function RemoveChessPieceCommand.at(position)
    MatchCommands.removeChessPieceAt(ChessBoardCommandExecutor, position)
end

return RemoveChessPieceCommand