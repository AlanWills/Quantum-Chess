local SetActiveColourCommand = require("Commands/SetActiveColourCommand.lua")

ChessBoard:clear()

-- Set up board
for y = 1, 8 do
    for x = 1, 8 do
        ChessBoard:addChessBoardTile(Vector2Int.new(x, y))
    end
end

-- Set up White Pawns
for xPos = 1, 8 do
    ChessBoard:addChessPiece(1, 
    {
        x = xPos,
        y = 2,
        colour = "White",
        type = "Pawn",
        verticalDirection = "Up"
    })
end

-- Set up White Rooks
ChessBoard:addChessPiece(1, 
{
    x = 1,
    y = 1,
    colour = "White",
    type = "Rook",
})

ChessBoard:addChessPiece(1, 
{
    x = 8,
    y = 1,
    colour = "White",
    type = "Rook"
})

-- Set Up White Knights
ChessBoard:addChessPiece(1, 
{
    x = 2,
    y = 1,
    colour = "White",
    type = "Knight"
})

ChessBoard:addChessPiece(1, 
{
    x = 7,
    y = 1,
    colour = "White",
    type = "Knight"
})

-- Set Up White Bishops
ChessBoard:addChessPiece(1, 
{
    x = 3,
    y = 1,
    colour = "White",
    type = "Bishop"
})

ChessBoard:addChessPiece(1, 
{
    x = 6,
    y = 1,
    colour = "White",
    type = "Bishop"
})

-- Set Up White Queen
ChessBoard:addChessPiece(1, 
{
    x = 4,
    y = 1,
    colour = "White",
    type = "Queen"
})

-- Set Up White King
ChessBoard:addChessPiece(1, 
{
    x = 5,
    y = 1,
    colour = "White",
    type = "King"
})

-- Set up Black Pawns
for xPos = 1, 8 do
    ChessBoard:addChessPiece(1, 
    {
        x = xPos,
        y = 7,
        colour = "Black",
        type = "Pawn",
        verticalDirection = "Down"
    })
end

-- Set up Black Rooks
ChessBoard:addChessPiece(1, 
{
    x = 1,
    y = 8,
    colour = "Black",
    type = "Rook"
})

ChessBoard:addChessPiece(1, 
{
    x = 8,
    y = 8,
    colour = "Black",
    type = "Rook"
})

-- Set Up Black Knights
ChessBoard:addChessPiece(1, 
{
    x = 2,
    y = 8,
    colour = "Black",
    type = "Knight"
})

ChessBoard:addChessPiece(1, 
{
    x = 7,
    y = 8,
    colour = "Black",
    type = "Knight"
})

-- Set Up Black Bishops
ChessBoard:addChessPiece(1, 
{
    x = 3,
    y = 8,
    colour = "Black",
    type = "Bishop"
})

ChessBoard:addChessPiece(1, 
{
    x = 6,
    y = 8,
    colour = "Black",
    type = "Bishop"
})

-- Set Up Black Queen
ChessBoard:addChessPiece(1, 
{
    x = 4,
    y = 8,
    colour = "Black",
    type = "Queen"
})

-- Set Up Black King
ChessBoard:addChessPiece(1, 
{
    x = 5,
    y = 8,
    colour = "Black",
    type = "King"
})

SetActiveColourCommand.toWhite()