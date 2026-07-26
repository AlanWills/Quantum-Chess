local StandardChessMatchFactory = require("Factories/StandardChessMatchFactory.lua")

local Scenario = { }

function Scenario:setUp()
    local boardWidth = 8
    local boardHeight = 8
    local startingXCoordinate = 1

    StandardChessMatchFactory.createBoard(boardHeight, boardWidth)
    StandardChessMatchFactory.createPieces(boardHeight, boardWidth, leftMostXPosition)
    StandardChessMatchFactory.setStartingPlayer("White")
end

function Scenario:load()
    StandardChessMatchFactory.createAIWhitePlayer({
        difficulty = "Medium"
    })
    StandardChessMatchFactory.createAIBlackPlayer({
        difficulty = "Easy"
    })
end

return Scenario