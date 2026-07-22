local AddBoardObjectCommand = { }

AddBoardObjectCommand.customisableChessPiece = 1
AddBoardObjectCommand.whitePawn = 2
AddBoardObjectCommand.whiteKnight = 3
AddBoardObjectCommand.whiteBishop = 4
AddBoardObjectCommand.whiteRook = 5
AddBoardObjectCommand.whiteQueen = 6
AddBoardObjectCommand.whiteKing = 7
AddBoardObjectCommand.blackPawn = 8
AddBoardObjectCommand.blackKnight = 9
AddBoardObjectCommand.blackBishop = 10
AddBoardObjectCommand.blackRook = 11
AddBoardObjectCommand.blackQueen = 12
AddBoardObjectCommand.blackKing = 13

function AddBoardObjectCommand.presetChessPiece(guid, config)
    MatchCommands.addChessPiece(BoardCommandExecutor, guid, config)
end

function AddBoardObjectCommand.customisedChessPiece(config)
    MatchCommands.addCustomisedChessPiece(BoardCommandExecutor, AddBoardObjectCommand.customisableChessPiece, config)
end

return AddBoardObjectCommand