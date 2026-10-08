local StandardChessMatchFactory = require("Factories/StandardChessMatchFactory.lua")
local AddBoardObjectCommand = require("Commands/AddBoardObjectCommand.lua")

local Scenario = { }

function Scenario:setUp()
    local boardHeight = 8
    local boardWidth = 8

    StandardChessMatchFactory.createBoard(boardWidth, boardHeight)

    -- Set Up Bishops
    AddBoardObjectCommand.presetPiece(14,
    { 
        position = Vector2Int.new(4, 1), 
        colour = "White",
        interactable = true
    })
    AddBoardObjectCommand.presetPiece(14, 
    { 
        position = Vector2Int.new(4, 8), 
        colour = "Black"
    })
    
    -- Set Up Kings
    AddBoardObjectCommand.presetPiece(AddBoardObjectCommand.whiteKing, { position = Vector2Int.new(1, 1) })
    AddBoardObjectCommand.presetPiece(AddBoardObjectCommand.blackKing, { position = Vector2Int.new(1, 8) })
    
    StandardChessMatchFactory.setStartingPlayer("White")
end

function Scenario:load()
    StandardChessMatchFactory.createHumanWhitePlayer()
    StandardChessMatchFactory.createAIBlackPlayer({
        difficulty = "Medium"    
    })
end

return Scenario