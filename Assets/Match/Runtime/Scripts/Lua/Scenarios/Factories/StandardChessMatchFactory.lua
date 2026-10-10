local AddBoardTileCommand = require("Commands/AddBoardTileCommand.lua")
local AddBoardObjectCommand = require("Commands/AddBoardObjectCommand.lua")
local SetActivePlayerCommand = require("Commands/SetActivePlayerCommand.lua")
local MakeAutomaticPromotionZoneCommand = require("Commands/MakeAutomaticPromotionZoneCommand.lua")

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

function StandardChessMatchFactory.createHumanBlackPlayer()
    MatchState:addPlayer(
    {
        colour = "Black",
        winCondition = "Checkmate",
        winConditionData = "White",
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

function StandardChessMatchFactory.createBoard(boardWidth, boardHeight)
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
    
    -- Black Promotion Zones
    for x = 1, boardWidth do
        local position = Vector2Int.new(x, 1)
        local blackPromotions = {
            {
                requiredPieceType = "Pawn",
                requiredPieceColour = "Black",
                promotedPieceType = "Queen"
            }
        }

        MakeAutomaticPromotionZoneCommand.new(position, blackPromotions)
    end
    
    -- White Promotion Zones
    for x = 1, boardWidth do
        local position = Vector2Int.new(x, boardHeight)
        local whitePromotions = {
            {
                requiredPieceType = "Pawn",
                requiredPieceColour = "White",
                promotedPieceType = "Queen"
            }
        }

        MakeAutomaticPromotionZoneCommand.new(position, whitePromotions)
    end
end

function StandardChessMatchFactory.createPieces(boardWidth, boardHeight, leftMostXPosition)
    local rightMostXPosition = leftMostXPosition + 7

    -- Set up White Pawns
    for xPos = leftMostXPosition, rightMostXPosition do
        AddBoardObjectCommand.presetPiece(AddBoardObjectCommand.whitePawn,
        {
            position = Vector2Int.new(xPos, 2),
            verticalDirection = "Up",
        })
    end

    -- Set up White Rooks
    AddBoardObjectCommand.presetPiece(AddBoardObjectCommand.whiteRook, { position = Vector2Int.new(leftMostXPosition, 1) })
    AddBoardObjectCommand.presetPiece(AddBoardObjectCommand.whiteRook, { position = Vector2Int.new(rightMostXPosition, 1) })

    -- Set Up White Knights
    AddBoardObjectCommand.presetPiece(AddBoardObjectCommand.whiteKnight, { position = Vector2Int.new(leftMostXPosition + 1, 1) })
    AddBoardObjectCommand.presetPiece(AddBoardObjectCommand.whiteKnight, { position = Vector2Int.new(rightMostXPosition - 1, 1) })

    -- Set Up White Bishops
    AddBoardObjectCommand.presetPiece(AddBoardObjectCommand.whiteBishop, { position = Vector2Int.new(leftMostXPosition + 2, 1) })
    AddBoardObjectCommand.presetPiece(AddBoardObjectCommand.whiteBishop, { position = Vector2Int.new(rightMostXPosition - 2, 1) })

    -- Set Up White Queen
    AddBoardObjectCommand.presetPiece(AddBoardObjectCommand.whiteQueen, { position = Vector2Int.new(leftMostXPosition + 3, 1) })

    -- Set Up White King
    AddBoardObjectCommand.presetPiece(AddBoardObjectCommand.whiteKing, { position = Vector2Int.new(rightMostXPosition - 3, 1) })

    -- Set up Black Pawns
    for xPos = leftMostXPosition, rightMostXPosition do
        AddBoardObjectCommand.presetPiece(AddBoardObjectCommand.blackPawn,
        {
            position = Vector2Int.new(xPos, boardHeight - 1),
            verticalDirection = "Down",
        })
    end

    -- Set up Black Rooks
    AddBoardObjectCommand.presetPiece(AddBoardObjectCommand.blackRook, { position = Vector2Int.new(leftMostXPosition, boardHeight) })
    AddBoardObjectCommand.presetPiece(AddBoardObjectCommand.blackRook, { position = Vector2Int.new(rightMostXPosition, boardHeight) })

    -- Set Up Black Knights
    AddBoardObjectCommand.presetPiece(AddBoardObjectCommand.blackKnight, { position = Vector2Int.new(leftMostXPosition + 1, boardHeight) })
    AddBoardObjectCommand.presetPiece(AddBoardObjectCommand.blackKnight, { position = Vector2Int.new(rightMostXPosition - 1, boardHeight) })

    -- Set Up Black Bishops
    AddBoardObjectCommand.presetPiece(AddBoardObjectCommand.blackBishop, { position = Vector2Int.new(leftMostXPosition + 2, boardHeight) })
    AddBoardObjectCommand.presetPiece(AddBoardObjectCommand.blackBishop, { position = Vector2Int.new(rightMostXPosition - 2, boardHeight) })

    -- Set Up Black Queen
    AddBoardObjectCommand.presetPiece(AddBoardObjectCommand.blackQueen, { position = Vector2Int.new(leftMostXPosition + 3, boardHeight) })

    -- Set Up Black King
    AddBoardObjectCommand.presetPiece(AddBoardObjectCommand.blackKing, { position = Vector2Int.new(rightMostXPosition - 3, boardHeight) })
end

function StandardChessMatchFactory.setStartingPlayer(startingColour)
    SetActivePlayerCommand.toColour(startingColour)
end

return StandardChessMatchFactory