local StandardChessMatchFactory = require("Factories/StandardChessMatchFactory.lua")

local Scenario = { }

function Scenario:setUp()
    local boardHeight = 10
    local boardWidth = 10
    local leftMostXPosition = 2
    
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