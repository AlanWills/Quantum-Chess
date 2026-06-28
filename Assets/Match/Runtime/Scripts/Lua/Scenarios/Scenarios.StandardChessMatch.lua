local AddChessBoardTileCommand = require("Commands/AddChessBoardTileCommand.lua")
local AddChessPieceCommand = require("Commands/AddChessPieceCommand.lua")
local SetActivePlayerCommand = require("Commands/SetActivePlayerCommand.lua")
local MoveCommand = require("Commands/MoveCommand.lua")

local Scenario = { }

function Scenario:load()
    MatchState:addPlayer(
    {
        colour = "White",
        winCondition = "Checkmate",
        winConditionData = "Black",
        turnStrategy = "Manual"
    })
    
    MatchState:addPlayer(
    {
        colour = "Black",
        winCondition = "Checkmate",
        winConditionData = "White",
        turnStrategy = "AI",
        turnStrategyData = 
        {
            depth = 2
        }
    })
end

function Scenario:setUp()
    -- Set up board
    for y = 1, 8 do
        for x = 1, 8 do
            local tileGuid = ((x + y + 1) % 2) + 1
            AddChessBoardTileCommand.new(tileGuid,
            {
                position = Vector2Int.new(x, y)
            })
        end
    end
    
    -- Set up White Pawns
    for xPos = 1, 8 do
        AddChessPieceCommand.preset(AddChessPieceCommand.whitePawn,
        {
            position = Vector2Int.new(xPos, 2),
            verticalDirection = "Up",
        })
    end
    
    -- Set up White Rooks
    AddChessPieceCommand.preset(AddChessPieceCommand.whiteRook, { position = Vector2Int.new(1, 1) })
    AddChessPieceCommand.preset(AddChessPieceCommand.whiteRook, { position = Vector2Int.new(8, 1) })
    
    -- Set Up White Knights
    AddChessPieceCommand.preset(AddChessPieceCommand.whiteKnight, { position = Vector2Int.new(2, 1) })
    AddChessPieceCommand.preset(AddChessPieceCommand.whiteKnight, { position = Vector2Int.new(7, 1) })
    
    -- Set Up White Bishops
    AddChessPieceCommand.preset(AddChessPieceCommand.whiteBishop, { position = Vector2Int.new(3, 1) })
    AddChessPieceCommand.preset(AddChessPieceCommand.whiteBishop, { position = Vector2Int.new(6, 1) })
    
    -- Set Up White Queen
    AddChessPieceCommand.preset(AddChessPieceCommand.whiteQueen, { position = Vector2Int.new(4, 1) })
    
    -- Set Up White King
    AddChessPieceCommand.preset(AddChessPieceCommand.whiteKing, { position = Vector2Int.new(5, 1) })
    
    -- Set up Black Pawns
    for xPos = 1, 8 do
        AddChessPieceCommand.preset(AddChessPieceCommand.blackPawn,
        {
            position = Vector2Int.new(xPos, 7),
            verticalDirection = "Down",
        })
    end
    
    -- Set up Black Rooks
    AddChessPieceCommand.preset(AddChessPieceCommand.blackRook, { position = Vector2Int.new(1, 8) })
    AddChessPieceCommand.preset(AddChessPieceCommand.blackRook, { position = Vector2Int.new(8, 8) })

    -- Set Up Black Knights
    AddChessPieceCommand.preset(AddChessPieceCommand.blackKnight, { position = Vector2Int.new(2, 8) })
    AddChessPieceCommand.preset(AddChessPieceCommand.blackKnight, { position = Vector2Int.new(7, 8) })

    -- Set Up Black Bishops
    AddChessPieceCommand.preset(AddChessPieceCommand.blackBishop, { position = Vector2Int.new(3, 8) })
    AddChessPieceCommand.preset(AddChessPieceCommand.blackBishop, { position = Vector2Int.new(6, 8) })

    -- Set Up Black Queen
    AddChessPieceCommand.preset(AddChessPieceCommand.blackQueen, { position = Vector2Int.new(4, 8) })

    -- Set Up Black King
    AddChessPieceCommand.preset(AddChessPieceCommand.blackKing, { position = Vector2Int.new(5, 8) })
    
    SetActivePlayerCommand.toColour("White")
end

return Scenario