local StandardChessMatchFactory = require("Factories/StandardChessMatchFactory.lua")

local Scenario = { }

function Scenario:setUp()
    local boardHeight = 8
    local boardWidth = 8
    local leftMostXPosition = 1

    StandardChessMatchFactory.createBoard(boardHeight, boardWidth, leftMostXPosition)
    StandardChessMatchFactory.setStartingPlayer("White")
end

function Scenario:load()
    StandardChessMatchFactory.createHumanWhitePlayer()
    StandardChessMatchFactory.createAIBlackPlayer({
        difficulty = "Easy"
    })
    
    -- Turn off auto queue of actions so we can debug AI calculations if needs be
    MatchState:setPlayerAutoQueueAction(1, false)
end

return Scenario