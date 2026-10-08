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
        difficulty = "Expert"    
    })
end

return Scenario