local StandardChessMatchFactory = require("Factories/StandardChessMatchFactory.lua")
local AddBoardObjectCommand = require("Commands/AddBoardObjectCommand.lua")

local Scenario = { }

function Scenario:setUp()
    local boardHeight = 8
    local boardWidth = 8

    StandardChessMatchFactory.createBoard(boardWidth, boardHeight)

    -- Set Up Pawns
    AddBoardObjectCommand.presetPiece(AddBoardObjectCommand.whitePawn,
    { 
        position = Vector2Int.new(3, 7),
        verticalDirection = "Up",
        interactable = true
    })
    AddBoardObjectCommand.presetPiece(AddBoardObjectCommand.blackPawn, 
    { 
        position = Vector2Int.new(5, 2),
        verticalDirection = "Down",
        interactable = true
    })
    
    -- Set Up Kings
    AddBoardObjectCommand.presetPiece(AddBoardObjectCommand.whiteKing, { position = Vector2Int.new(1, 1) })
    AddBoardObjectCommand.presetPiece(AddBoardObjectCommand.blackKing, { position = Vector2Int.new(1, 8) })
    
    StandardChessMatchFactory.setStartingPlayer("White")
end

function Scenario:load()
    StandardChessMatchFactory.createHumanWhitePlayer()
    StandardChessMatchFactory.createHumanBlackPlayer()
end

return Scenario