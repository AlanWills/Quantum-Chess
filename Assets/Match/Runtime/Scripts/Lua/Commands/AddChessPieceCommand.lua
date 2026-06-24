local AddChessPieceCommand = { }

AddChessPieceCommand.customisableChessPiece = 1
AddChessPieceCommand.whitePawn = 2
AddChessPieceCommand.whiteKnight = 3
AddChessPieceCommand.whiteBishop = 4
AddChessPieceCommand.whiteRook = 5
AddChessPieceCommand.whiteQueen = 6
AddChessPieceCommand.whiteKing = 7
AddChessPieceCommand.blackPawn = 8
AddChessPieceCommand.blackKnight = 9
AddChessPieceCommand.blackBishop = 10
AddChessPieceCommand.blackRook = 11
AddChessPieceCommand.blackQueen = 12
AddChessPieceCommand.blackKing = 13

function AddChessPieceCommand.preset(guid, config)
    MatchCommands.addChessPiece(ChessBoardCommandExecutor, guid, config)
end

function AddChessPieceCommand.customised(config)
    MatchCommands.addChessPiece(ChessBoardCommandExecutor, AddChessPieceCommand.customisableChessPiece, config)
end

return AddChessPieceCommand