local StandardChessMatchFactory = require("Factories/StandardChessMatchFactory.lua")
local CampaignFactory = require("Factories/CampaignFactory.lua")

local Scenario = { }

function Scenario.setUp()
    local boardHeight = 8
    local boardWidth = 8
    local deploymentZones =
    {
        Vector2Int.new(3, 1),
        Vector2Int.new(4, 1),
        Vector2Int.new(5, 1),
        Vector2Int.new(6, 1),
        Vector2Int.new(3, 2),
        Vector2Int.new(4, 2),
        Vector2Int.new(5, 2),
        Vector2Int.new(6, 2)
    }

    -- Create Board
    StandardChessMatchFactory.createBoard(boardHeight, boardWidth)
    
    -- Add Enemy Units
    StandardChessMatchFactory.addBlackBishop(Vector2Int.new(3, 8))
    StandardChessMatchFactory.addBlackKing(Vector2Int.new(4, 8))
    StandardChessMatchFactory.addBlackQueen(Vector2Int.new(5, 8))
    StandardChessMatchFactory.addBlackBishop(Vector2Int.new(6, 8))
    StandardChessMatchFactory.addBlackPawn(Vector2Int.new(3, 7))
    StandardChessMatchFactory.addBlackPawn(Vector2Int.new(4, 7))
    StandardChessMatchFactory.addBlackPawn(Vector2Int.new(5, 7))
    StandardChessMatchFactory.addBlackPawn(Vector2Int.new(6, 7))
    
    -- Load Player's Units
    CampaignFactory.loadPlayerCurrentUnits(deploymentZones)
    
    -- Set Starting Player
    StandardChessMatchFactory.setStartingPlayer("White")
end

function Scenario.load()
    StandardChessMatchFactory.createHumanWhitePlayer()
    StandardChessMatchFactory.createAIBlackPlayer({
        difficulty = "Hard"
    })
end

return Scenario