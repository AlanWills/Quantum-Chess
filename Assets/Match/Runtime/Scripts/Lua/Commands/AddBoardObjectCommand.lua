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

function AddBoardObjectCommand.presetPiece(guid, config)
    MatchCommands.addPiece(BoardCommandExecutor, guid, config)
end

function AddBoardObjectCommand.customisedPiece(config)
    MatchCommands.addCustomisedPiece(BoardCommandExecutor, AddBoardObjectCommand.customisableChessPiece, config)
end

return AddBoardObjectCommand