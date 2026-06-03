local ClearChessBoardCommand = { }

function ClearChessBoardCommand.new()
    MatchCommands.clearChessBoard(ChessBoardCommandExecutor)
end

return ClearChessBoardCommand