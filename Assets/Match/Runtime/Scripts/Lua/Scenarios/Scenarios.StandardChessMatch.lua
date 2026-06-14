local AddChessPieceCommand = require("Commands/AddChessPieceCommand.lua")
local ClearChessBoardCommand = require("Commands/ClearChessBoardCommand.lua")
local SetActivePlayerCommand = require("Commands/SetActivePlayerCommand.lua")
local EndTurnCommand = require("Commands/EndTurnCommand.lua")
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
        turnStrategy = "Custom",
        onTurnBegin = function()
            -- TODO: Add actual logic here
            
            -- TODO: Delete when we do proper logic
            MoveCommand.fromTo(Vector2Int.new(4, 7), Vector2Int.new(4, 5))
        end,
        onTurnEnd = function()
            print("Black Turn End!")
        end
    })
end

function Scenario:setUp()
    ClearChessBoardCommand.new()
    
    -- Set up board
    for y = 1, 8 do
        for x = 1, 8 do
            ChessBoard:addChessBoardTile(Vector2Int.new(x, y))
        end
    end
    
    -- Set up White Pawns
    for xPos = 1, 8 do
        AddChessPieceCommand.new(1, 
        {
            position = Vector2Int.new(xPos, 2),
            colour = "White",
            type = "Pawn",
            verticalDirection = "Up",
            interactable = true
        })
    end
    
    -- Set up White Rooks
    AddChessPieceCommand.new(1, 
    {
        position = Vector2Int.new(1, 1),
        colour = "White",
        type = "Rook",
        interactable = true
    })
    
    AddChessPieceCommand.new(1, 
    {
        position = Vector2Int.new(8, 1),
        colour = "White",
        type = "Rook",
        interactable = true
    })
    
    -- Set Up White Knights
    AddChessPieceCommand.new(1, 
    {
        position = Vector2Int.new(2, 1),
        colour = "White",
        type = "Knight",
        interactable = true
    })
    
    AddChessPieceCommand.new(1, 
    {
        position = Vector2Int.new(7, 1),
        colour = "White",
        type = "Knight",
        interactable = true
    })
    
    -- Set Up White Bishops
    AddChessPieceCommand.new(1, 
    {
        position = Vector2Int.new(3, 1),
        colour = "White",
        type = "Bishop",
        interactable = true
    })
    
    AddChessPieceCommand.new(1, 
    {
        position = Vector2Int.new(6, 1),
        colour = "White",
        type = "Bishop",
        interactable = true
    })
    
    -- Set Up White Queen
    AddChessPieceCommand.new(1, 
    {
        position = Vector2Int.new(4, 1),
        colour = "White",
        type = "Queen",
        interactable = true
    })
    
    -- Set Up White King
    AddChessPieceCommand.new(1, 
    {
        position = Vector2Int.new(5, 1),
        colour = "White",
        type = "King",
        interactable = true
    })
    
    -- Set up Black Pawns
    for xPos = 1, 8 do
        AddChessPieceCommand.new(2, 
        {
            position = Vector2Int.new(xPos, 7),
            colour = "Black",
            type = "Pawn",
            verticalDirection = "Down"
        })
    end
    
    -- Set up Black Rooks
    AddChessPieceCommand.new(2, 
    {
        position = Vector2Int.new(1, 8),
        colour = "Black",
        type = "Rook"
    })
    
    AddChessPieceCommand.new(2, 
    {
        position = Vector2Int.new(8, 8),
        colour = "Black",
        type = "Rook"
    })
    
    -- Set Up Black Knights
    AddChessPieceCommand.new(2, 
    {
        position = Vector2Int.new(2, 8),
        colour = "Black",
        type = "Knight"
    })
    
    AddChessPieceCommand.new(2, 
    {
        position = Vector2Int.new(7, 8),
        colour = "Black",
        type = "Knight"
    })
    
    -- Set Up Black Bishops
    AddChessPieceCommand.new(2, 
    {
        position = Vector2Int.new(3, 8),
        colour = "Black",
        type = "Bishop"
    })
    
    AddChessPieceCommand.new(2, 
    {
        position = Vector2Int.new(6, 8),
        colour = "Black",
        type = "Bishop"
    })
    
    -- Set Up Black Queen
    AddChessPieceCommand.new(2, 
    {
        position = Vector2Int.new(4, 8),
        colour = "Black",
        type = "Queen"
    })
    
    -- Set Up Black King
    AddChessPieceCommand.new(2, 
    {
        position = Vector2Int.new(5, 8),
        colour = "Black",
        type = "King"
    })
    
    SetActivePlayerCommand.toColour("White")
end

return Scenario