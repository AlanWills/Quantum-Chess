local CheckWinConditionForPlayer = require("Commands/CheckPlayerWinConditionCommand.lua")
local DryRunNextActionForPlayer = require("Commands/DryRunNextActionForPlayerCommand.lua")

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
    
    self:drawPlayers()
end

function MatchStateDebugMenu:drawPlayers()
    for playerIndex = 0, MatchState.numPlayers - 1, 1 do
        GUILayout.beginSection("Player " .. tostring(playerIndex))
            local playerColour = MatchState:getPlayerColour(playerIndex)
            local autoEndTurn = MatchState:getPlayerAutoEndTurn(playerIndex)
            
            GUILayout.label("Colour: " .. playerColour)
    
            local newAutoEndTurn = GUILayout.toggle("Auto End Turn:", autoEndTurn)
            if autoEndTurn ~= newAutoEndTurn then
                MatchState:setPlayerAutoEndTurn(playerIndex, autoEndTurn)
            end
            
            GUILayout.beginHorizontal()
    
            if GUILayout.button("Dry Run Next Action") then
                DryRunNextActionForPlayer.new(playerIndex)
            end
            
            if GUILayout.button("Check Win Condition") then
                CheckWinConditionForPlayer.new(playerIndex)
            end
            
            GUILayout.endHorizontal()
        GUILayout.endSection();
    end
end

return MatchStateDebugMenu