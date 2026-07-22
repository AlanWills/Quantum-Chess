local AddBoardTileCommand = require("Commands/AddBoardTileCommand.lua")
local AddBoardObjectCommand = require("Commands/AddBoardObjectCommand.lua")
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

            AddBoardTileCommand.new(tileGuid,
            {
                position = position,
                importance = importance
            })
        end
    end

    -- Set up White Pawns
    for xPos = leftMostXPosition, rightMostXPosition do
        AddBoardObjectCommand.presetChessPiece(AddBoardObjectCommand.whitePawn,
        {
            position = Vector2Int.new(xPos, 2),
            verticalDirection = "Up",
        })
    end

    -- Set up White Rooks
    AddBoardObjectCommand.presetChessPiece(AddBoardObjectCommand.whiteRook, { position = Vector2Int.new(leftMostXPosition, 1) })
    AddBoardObjectCommand.presetChessPiece(AddBoardObjectCommand.whiteRook, { position = Vector2Int.new(rightMostXPosition, 1) })

    -- Set Up White Knights
    AddBoardObjectCommand.presetChessPiece(AddBoardObjectCommand.whiteKnight, { position = Vector2Int.new(leftMostXPosition + 1, 1) })
    AddBoardObjectCommand.presetChessPiece(AddBoardObjectCommand.whiteKnight, { position = Vector2Int.new(rightMostXPosition - 1, 1) })

    -- Set Up White Bishops
    AddBoardObjectCommand.presetChessPiece(AddBoardObjectCommand.whiteBishop, { position = Vector2Int.new(leftMostXPosition + 2, 1) })
    AddBoardObjectCommand.presetChessPiece(AddBoardObjectCommand.whiteBishop, { position = Vector2Int.new(rightMostXPosition - 2, 1) })

    -- Set Up White Queen
    AddBoardObjectCommand.presetChessPiece(AddBoardObjectCommand.whiteQueen, { position = Vector2Int.new(leftMostXPosition + 3, 1) })

    -- Set Up White King
    AddBoardObjectCommand.presetChessPiece(AddBoardObjectCommand.whiteKing, { position = Vector2Int.new(rightMostXPosition - 3, 1) })

    -- Set up Black Pawns
    for xPos = leftMostXPosition, rightMostXPosition do
        AddBoardObjectCommand.presetChessPiece(AddBoardObjectCommand.blackPawn,
        {
            position = Vector2Int.new(xPos, boardHeight - 1),
            verticalDirection = "Down",
        })
    end

    -- Set up Black Rooks
    AddBoardObjectCommand.presetChessPiece(AddBoardObjectCommand.blackRook, { position = Vector2Int.new(leftMostXPosition, boardHeight) })
    AddBoardObjectCommand.presetChessPiece(AddBoardObjectCommand.blackRook, { position = Vector2Int.new(rightMostXPosition, boardHeight) })

    -- Set Up Black Knights
    AddBoardObjectCommand.presetChessPiece(AddBoardObjectCommand.blackKnight, { position = Vector2Int.new(leftMostXPosition + 1, boardHeight) })
    AddBoardObjectCommand.presetChessPiece(AddBoardObjectCommand.blackKnight, { position = Vector2Int.new(rightMostXPosition - 1, boardHeight) })

    -- Set Up Black Bishops
    AddBoardObjectCommand.presetChessPiece(AddBoardObjectCommand.blackBishop, { position = Vector2Int.new(leftMostXPosition + 2, boardHeight) })
    AddBoardObjectCommand.presetChessPiece(AddBoardObjectCommand.blackBishop, { position = Vector2Int.new(rightMostXPosition - 2, boardHeight) })

    -- Set Up Black Queen
    AddBoardObjectCommand.presetChessPiece(AddBoardObjectCommand.blackQueen, { position = Vector2Int.new(leftMostXPosition + 3, boardHeight) })

    -- Set Up Black King
    AddBoardObjectCommand.presetChessPiece(AddBoardObjectCommand.blackKing, { position = Vector2Int.new(rightMostXPosition - 3, boardHeight) })
end

function StandardChessMatchFactory.setStartingPlayer(startingColour)
    SetActivePlayerCommand.toColour(startingColour)
end

return StandardChessMatchFactory