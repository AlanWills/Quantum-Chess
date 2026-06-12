local MatchScenariosDebugMenu = { }

MatchScenariosDebugMenu.currentScenarioPage = 0
MatchScenariosDebugMenu.scenarioEntriesPerPage = 20

function MatchScenariosDebugMenu:onDrawMenu()
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
end

return MatchScenariosDebugMenu