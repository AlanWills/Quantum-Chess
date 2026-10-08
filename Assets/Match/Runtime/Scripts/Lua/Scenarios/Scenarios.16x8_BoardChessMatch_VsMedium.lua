local StandardChessMatchFactory = require("Factories/StandardChessMatchFactory.lua")

local Scenario = { }

function Scenario:setUp()
    local boardHeight = 8
    local boardWidth = 16
    local leftMostXPosition = 5

    StandardChessMatchFactory.createBoard(boardWidth, boardHeight)
    StandardChessMatchFactory.createPieces(boardWidth, boardHeight, leftMostXPosition)
    StandardChessMatchFactory.setStartingPlayer("White")
end

function Scenario:load()
    StandardChessMatchFactory.createHumanWhitePlayer()
    StandardChessMatchFactory.createAIBlackPlayer({
        difficulty = "Medium"
    })
end

return Scenario