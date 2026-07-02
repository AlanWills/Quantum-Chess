local AddChessBoardTileCommand = require("Commands/AddChessBoardTileCommand.lua")
local AddChessPieceCommand = require("Commands/AddChessPieceCommand.lua")
local SetActivePlayerCommand = require("Commands/SetActivePlayerCommand.lua")

local Scenario = { }

function Scenario:load()
    MatchState:addPlayer(
    {
        colour = "White",
        winCondition = "Checkmate",
        winConditionData = "Black",
        turnStrategy = "AlphaBeta",
        turnStrategyData = {
            depth = 4
        }
    })

    MatchState:addPlayer(
    {
        colour = "Black",
        winCondition = "Checkmate",
        winConditionData = "White",
        turnStrategy = "AlphaBeta",
        turnStrategyData = {
            depth = 2
        }
    })
end

function Scenario:setUp()
    -- Set up board
    local boardHeight = 8
    local boardWidth = 8
    local leftMostXPosition = 1
    local rightMostXPosition = 8

    for y = 1, boardHeight do
        for x = 1, boardWidth do
            local tileGuid = ((x + y + 1) % 2) + 1
            AddChessBoardTileCommand.new(tileGuid,
            {
                position = Vector2Int.new(x, y)
            })
        end
    end

    -- Set up White Pawns
    for xPos = leftMostXPosition, rightMostXPosition do
        AddChessPieceCommand.preset(AddChessPieceCommand.whitePawn,
        {
            position = Vector2Int.new(xPos, 2),
            verticalDirection = "Up",
        })
    end

    -- Set up White Rooks
    AddChessPieceCommand.preset(AddChessPieceCommand.whiteRook, { position = Vector2Int.new(leftMostXPosition, 1) })
    AddChessPieceCommand.preset(AddChessPieceCommand.whiteRook, { position = Vector2Int.new(rightMostXPosition, 1) })

    -- Set Up White Knights
    AddChessPieceCommand.preset(AddChessPieceCommand.whiteKnight, { position = Vector2Int.new(leftMostXPosition + 1, 1) })
    AddChessPieceCommand.preset(AddChessPieceCommand.whiteKnight, { position = Vector2Int.new(rightMostXPosition - 1, 1) })

    -- Set Up White Bishops
    AddChessPieceCommand.preset(AddChessPieceCommand.whiteBishop, { position = Vector2Int.new(leftMostXPosition + 2, 1) })
    AddChessPieceCommand.preset(AddChessPieceCommand.whiteBishop, { position = Vector2Int.new(rightMostXPosition - 2, 1) })

    -- Set Up White Queen
    AddChessPieceCommand.preset(AddChessPieceCommand.whiteQueen, { position = Vector2Int.new(leftMostXPosition + 3, 1) })

    -- Set Up White King
    AddChessPieceCommand.preset(AddChessPieceCommand.whiteKing, { position = Vector2Int.new(rightMostXPosition - 3, 1) })

    -- Set up Black Pawns
    for xPos = leftMostXPosition, rightMostXPosition do
        AddChessPieceCommand.preset(AddChessPieceCommand.blackPawn,
        {
            position = Vector2Int.new(xPos, boardHeight - 1),
            verticalDirection = "Down",
        })
    end

    -- Set up Black Rooks
    AddChessPieceCommand.preset(AddChessPieceCommand.blackRook, { position = Vector2Int.new(leftMostXPosition, boardHeight) })
    AddChessPieceCommand.preset(AddChessPieceCommand.blackRook, { position = Vector2Int.new(rightMostXPosition, boardHeight) })

    -- Set Up Black Knights
    AddChessPieceCommand.preset(AddChessPieceCommand.blackKnight, { position = Vector2Int.new(leftMostXPosition + 1, boardHeight) })
    AddChessPieceCommand.preset(AddChessPieceCommand.blackKnight, { position = Vector2Int.new(rightMostXPosition - 1, boardHeight) })

    -- Set Up Black Bishops
    AddChessPieceCommand.preset(AddChessPieceCommand.blackBishop, { position = Vector2Int.new(leftMostXPosition + 2, boardHeight) })
    AddChessPieceCommand.preset(AddChessPieceCommand.blackBishop, { position = Vector2Int.new(rightMostXPosition - 2, boardHeight) })

    -- Set Up Black Queen
    AddChessPieceCommand.preset(AddChessPieceCommand.blackQueen, { position = Vector2Int.new(leftMostXPosition + 3, boardHeight) })

    -- Set Up Black King
    AddChessPieceCommand.preset(AddChessPieceCommand.blackKing, { position = Vector2Int.new(rightMostXPosition - 3, boardHeight) })

    SetActivePlayerCommand.toColour("White")
end

return Scenario