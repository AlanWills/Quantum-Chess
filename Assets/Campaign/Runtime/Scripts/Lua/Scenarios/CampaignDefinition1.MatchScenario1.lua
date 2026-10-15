local StandardChessMatchFactory = require("Factories/StandardChessMatchFactory.lua")
local CampaignFactory = require("Factories/CampaignFactory.lua")

local Scenario = { }

function Scenario:setUp()
    local boardHeight = 8
    local boardWidth = 8
    local deploymentZones =
    {
        Vector2Int.new(4, 1),
        Vector2Int.new(5, 1)
    }

    StandardChessMatchFactory.createBoard(boardHeight, boardWidth)
    CampaignFactory.loadPlayerCurrentUnits(deploymentZones)
    StandardChessMatchFactory.setStartingPlayer("White")
end

function Scenario:load()
    StandardChessMatchFactory.createHumanWhitePlayer()
    StandardChessMatchFactory.createAIBlackPlayer({
        difficulty = "Medium"
    })
end

return Scenario