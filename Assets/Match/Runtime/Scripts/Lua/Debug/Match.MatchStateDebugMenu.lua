local MatchStateDebugMenu = { }

function MatchStateDebugMenu:onDrawMenu()
    GUILayout.label("Active Colour: " .. tostring(MatchState.activeColour))
    GUILayout.label("Next Active Colour: " .. tostring(MatchState.nextActiveColour))
end

return MatchStateDebugMenu