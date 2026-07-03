local StandardChessMatchFactory = require("Factories/StandardChessMatchFactory.lua")

local Scenario = { }

function Scenario:load()
    StandardChessMatchFactory.createAIWhitePlayer({
        depth = 4
    })
    StandardChessMatchFactory.createAIBlackPlayer({
        depth = 2
    })
end

function Scenario:setUp()
    local boardWidth = 8
    local boardHeight = 8
    local startingXCoordinate = 1
    
    StandardChessMatchFactory.createBoard(boardWidth, boardHeight, startingXCoordinate)
    StandardChessMatchFactory.setStartingPlayer("White")
end

return Scenario