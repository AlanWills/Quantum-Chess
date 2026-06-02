local MoveCommand = require("Commands/MoveCommand.lua")
local AttackCommand = require("Commands/AttackCommand.lua")
local SelectCommand = require("Commands/SelectCommand.lua")
local EnableInputCommand = require("Commands/EnableInputCommand.lua") 
local SetActiveColourCommand = require("Commands/SetActiveColourCommand.lua")

local MatchCommandsDebugMenu = { }
MatchCommandsDebugMenu.fromMovePosition = Vector2Int.new(0, 0)
MatchCommandsDebugMenu.toMovePosition = Vector2Int.new(1, 1)
MatchCommandsDebugMenu.fromAttackPosition = Vector2Int.new(0, 0)
MatchCommandsDebugMenu.toAttackPosition = Vector2Int.new(1, 1)
MatchCommandsDebugMenu.selectPosition = Vector2Int.new(0, 0)

function MatchCommandsDebugMenu:onDrawMenu()
    GUILayout.beginSection("Move")
        self.fromMovePosition = GUILayout.vector2IntField("From", self.fromMovePosition)
        self.toMovePosition = GUILayout.vector2IntField("To", self.toMovePosition)
        
        GUILayout.beginHorizontal()
            if GUILayout.button("From To") then
                MoveCommand.fromTo(self.fromMovePosition, self.toMovePosition)
            end
        
            if GUILayout.button("Selected To") then
                MoveCommand.selectedTo(self.toMovePosition)
            end
        GUILayout.endHorizontal()
    GUILayout.endSection()
    
    GUILayout.beginSection("Attack")
        self.fromAttackPosition = GUILayout.vector2IntField("From", self.fromAttackPosition)
        self.toAttackPosition = GUILayout.vector2IntField("To", self.toAttackPosition)
        
        GUILayout.beginHorizontal()
            if GUILayout.button("From To") then
                AttackCommand.fromTo(self.fromAttackPosition, self.toAttackPosition)
            end
        
            if GUILayout.button("Selected To") then
                AttackCommand.selectedTo(self.toAttackPosition)
            end
        GUILayout.endHorizontal()
    GUILayout.endSection()
        
    GUILayout.beginSection("Select")
        self.selectPosition = GUILayout.vector2IntField("Position", self.selectPosition)
        
        GUILayout.beginHorizontal()
            if GUILayout.button("Select") then
                SelectCommand.at(self.selectPosition)
            end
        
            if GUILayout.button("Clear Selection") then
                SelectCommand.clear()
            end
        GUILayout.endHorizontal()
    GUILayout.endSection()

    GUILayout.beginSection("Set Active Colour")
        GUILayout.beginHorizontal()
            if GUILayout.button("White") then
                SetActiveColourCommand.toWhite()
            end
        
            if GUILayout.button("Black") then
                EnableInputCommand.toBlack()
            end
        
            if GUILayout.button("Switch") then
                EnableInputCommand.switch()
            end
        GUILayout.endHorizontal()
    GUILayout.endSection()
    
    GUILayout.beginSection("Enable Input")
        GUILayout.beginHorizontal()
            if GUILayout.button("White") then
                EnableInputCommand.forWhite()
            end
            
            if GUILayout.button("Black") then
                EnableInputCommand.forBlack()
            end
        GUILayout.endHorizontal()
    GUILayout.endSection()
end

return MatchCommandsDebugMenu