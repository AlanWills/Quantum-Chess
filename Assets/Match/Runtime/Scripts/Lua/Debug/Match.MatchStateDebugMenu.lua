local CheckWinConditionForPlayer = require("Commands/CheckPlayerWinConditionCommand.lua")
local ExecuteNextActionForPlayer = require("Commands/ExecuteNextActionForPlayerCommand.lua")

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
            local autoQueueAction = MatchState:getPlayerAutoQueueAction(playerIndex);
            local autoEndTurn = MatchState:getPlayerAutoEndTurn(playerIndex)
            
            GUILayout.label("Colour: " .. playerColour)
    
            local newAutoQueueAction = GUILayout.toggle("Auto Queue Action:", autoQueueAction)
            if autoQueueAction ~= newAutoQueueAction then
                MatchState:setPlayerAutoQueueAction(playerIndex, newAutoQueueAction)
            end
            
            local newAutoEndTurn = GUILayout.toggle("Auto End Turn:", autoEndTurn)
            if autoEndTurn ~= newAutoEndTurn then
                MatchState:setPlayerAutoEndTurn(playerIndex, newAutoEndTurn)
            end
            
            GUILayout.beginHorizontal()
    
            if GUILayout.button("Dry Run Next Action") then
                ExecuteNextActionForPlayer.dryRun(playerIndex)
            end
            
            if GUILayout.button("Execute Next Action") then
                ExecuteNextActionForPlayer.new(playerIndex)
            end
            
            GUILayout.endHorizontal()
            
            if GUILayout.button("Check Win Condition") then
                CheckWinConditionForPlayer.new(playerIndex)
            end
            
        GUILayout.endSection();
    end
end

return MatchStateDebugMenu