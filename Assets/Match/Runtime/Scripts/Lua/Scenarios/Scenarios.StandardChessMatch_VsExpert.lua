local StandardChessMatchFactory = require("Factories/StandardChessMatchFactory.lua")

local Scenario = { }

function Scenario:load()
    StandardChessMatchFactory.createHumanWhitePlayer()
    StandardChessMatchFactory.createAIBlackPlayer({
        difficulty = "Expert"    
    })
end

function Scenario:setUp()
    local boardHeight = 8
    local boardWidth = 8
    local leftMostXPosition = 1

    StandardChessMatchFactory.createBoard(boardHeight, boardWidth, leftMostXPosition)
    StandardChessMatchFactory.setStartingPlayer("White")
end

return Scenario