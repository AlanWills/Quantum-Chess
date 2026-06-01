namespace QC.Match.Events
{
    public readonly struct ActiveColourChangedArgs
    {
        public readonly ChessPieceColour OldColour;
        public readonly ChessPieceColour NewColour;

        public ActiveColourChangedArgs(ChessPieceColour oldColour, ChessPieceColour newColour)
        {
            OldColour = oldColour;
            NewColour = newColour;
        }
    }
}