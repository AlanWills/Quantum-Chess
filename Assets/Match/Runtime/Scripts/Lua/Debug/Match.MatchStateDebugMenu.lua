local MatchStateDebugMenu = { }

MatchStateDebugMenu.currentScenarioPage = 0
MatchStateDebugMenu.scenarioEntriesPerPage = 20

function MatchStateDebugMenu:onDrawMenu()
    GUILayout.label("Active Scenario: " .. MatchState.activeScenarioGuid)
    GUILayout.label("Active Player Index: " .. tostring(MatchState.activePlayerIndex))
    GUILayout.label("Active Colour: " .. tostring(MatchState.activeColour))
    
    GUILayout.beginSection("Scenarios")
        self.currentScenarioPage = GUILayout.readOnlyPaginatedListField(
            self.currentScenarioPage,
            self.scenarioEntriesPerPage,
            MatchScenarioCatalogue.numScenarios,
            function(index)
                local scenario = MatchScenarioCatalogue:getItem(index)
                GUILayout.beginSection(scenario.name)
                    GUILayout.beginHorizontal()
                        GUILayout.label(scenario.guid)
                        
                        if GUILayout.button("Start") then
                            MatchState:startScenario(scenario.guid)
                        end
                    GUILayout.endHorizontal()
                GUILayout.endSection()
            end)
    GUILayout.endSection()
end

return MatchStateDebugMenu