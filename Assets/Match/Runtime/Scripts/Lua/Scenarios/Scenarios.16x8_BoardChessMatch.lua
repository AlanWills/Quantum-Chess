local StandardChessMatchFactory = require("Factories/StandardChessMatchFactory.lua")

local Scenario = { }

function Scenario:load()
    StandardChessMatchFactory.createHumanWhitePlayer()
    StandardChessMatchFactory.createAIBlackPlayer()
end

function Scenario:setUp()
    local boardHeight = 8
    local boardWidth = 16
    local leftMostXPosition = 5

    StandardChessMatchFactory.createBoard(boardHeight, boardWidth, leftMostXPosition)
    StandardChessMatchFactory.setStartingPlayer("White")
end

return Scenario