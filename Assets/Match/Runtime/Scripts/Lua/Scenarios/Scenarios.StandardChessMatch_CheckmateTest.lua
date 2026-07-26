local StandardChessMatchFactory = require("Factories/StandardChessMatchFactory.lua")
local AddBoardObjectCommand = require("Commands/AddBoardObjectCommand.lua")

local Scenario = { }

function Scenario:setUp()
    local boardHeight = 8
    local boardWidth = 8

    StandardChessMatchFactory.createBoard(boardHeight, boardWidth)
    
    AddBoardObjectCommand.presetChessPiece(AddBoardObjectCommand.whiteQueen, { position = Vector2Int.new(2, 1) })
    AddBoardObjectCommand.presetChessPiece(AddBoardObjectCommand.whiteQueen, { position = Vector2Int.new(3, 1) })
    AddBoardObjectCommand.presetChessPiece(AddBoardObjectCommand.whiteKing, { position = Vector2Int.new(4, 1) })
    AddBoardObjectCommand.presetChessPiece(AddBoardObjectCommand.blackKing, { position = Vector2Int.new(1, 8) })
    
    StandardChessMatchFactory.setStartingPlayer("White")
end

function Scenario:load()
    StandardChessMatchFactory.createHumanWhitePlayer()
    StandardChessMatchFactory.createAIBlackPlayer({
        difficulty = "Medium"    
    })
end

return Scenario