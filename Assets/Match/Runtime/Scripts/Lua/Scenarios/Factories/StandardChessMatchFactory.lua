local AddChessBoardTileCommand = require("Commands/AddChessBoardTileCommand.lua")
local AddChessPieceCommand = require("Commands/AddChessPieceCommand.lua")
local SetActivePlayerCommand = require("Commands/SetActivePlayerCommand.lua")

local StandardChessMatchFactory = {}

function StandardChessMatchFactory.createHumanWhitePlayer()
    MatchState:addPlayer(
    {
        colour = "White",
        winCondition = "Checkmate",
        winConditionData = "Black",
        turnStrategy = "Manual"
    })
end

function StandardChessMatchFactory.createAIWhitePlayer(turnStrategyData)
    MatchState:addPlayer(
    {
        colour = "White",
        winCondition = "Checkmate",
        winConditionData = "Black",
        turnStrategy = "AlphaBeta",
        turnStrategyData = turnStrategyData
    })
end

function StandardChessMatchFactory.createAIBlackPlayer(turnStrategyData)
    MatchState:addPlayer(
    {
        colour = "Black",
        winCondition = "Checkmate",
        winConditionData = "White",
        turnStrategy = "AlphaBeta",
        turnStrategyData = turnStrategyData
    })
end

function StandardChessMatchFactory.createBoard(boardWidth, boardHeight, leftMostXPosition)
    local rightMostXPosition = leftMostXPosition + 7

    for y = 1, boardHeight do
        for x = 1, boardWidth do
            local tileGuid = ((x + y + 1) % 2) + 1
            local position = Vector2Int.new(x, y)
            local importance = 0

            if (x == 4 or x == 5) and (y == 4 or y == 5) then
                importance = 2
            elseif (x == 3 or x == 6) and (x == 3 or x == 6) then
                importance = 1
            end
            
            AddChessBoardTileCommand.new(tileGuid,
            {
                position = position,
                importance = importance
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
end

function StandardChessMatchFactory.setStartingPlayer(startingColour)
    SetActivePlayerCommand.toColour(startingColour)
end

return StandardChessMatchFactory