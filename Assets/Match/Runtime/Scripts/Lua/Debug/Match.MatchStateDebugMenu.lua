local MatchStateDebugMenu = { }

function MatchStateDebugMenu:onDrawMenu()
    if MatchState.hasActiveScenario and MatchState.numPlayers > 0 then
        GUILayout.label("Active Scenario: " .. MatchState.activeScenarioGuid)
        GUILayout.label("Active Player Index: " .. tostring(MatchState.activePlayerIndex))
        GUILayout.label("Active Colour: " .. tostring(MatchState.activeColour))
        GUILayout.label("Next Player Index: " .. tostring(MatchState.nextPlayerIndex))
    else
        GUILayout.label("Has Active Scenario: " .. tostring(MatchState.hasActiveScenario))
        GUILayout.label("Num Players: " .. tostring(MatchState.numPlayers))
    end
end

return MatchStateDebugMenu