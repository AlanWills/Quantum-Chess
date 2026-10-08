local StandardChessMatchFactory = require("Factories/StandardChessMatchFactory.lua")

local Scenario = { }

function Scenario:setUp()
    local boardHeight = 8
    local boardWidth = 8
    local leftMostXPosition = 1

    StandardChessMatchFactory.createBoard(boardWidth, boardHeight)
    StandardChessMatchFactory.createPieces(boardWidth, boardHeight, leftMostXPosition)
    StandardChessMatchFactory.setStartingPlayer("White")
end

function Scenario:load()
    StandardChessMatchFactory.createHumanWhitePlayer()
    StandardChessMatchFactory.createAIBlackPlayer({
        difficulty = "Easy"
    })
    
    -- Turn off auto thinking so we can debug AI calculations if needs be
    MatchState:setPlayerAutoThink(1, false)
end

return Scenario