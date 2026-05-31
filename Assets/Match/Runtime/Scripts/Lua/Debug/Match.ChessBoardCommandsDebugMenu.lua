local MoveCommand = require("Commands/MoveCommand.lua")
local AttackCommand = require("Commands/AttackCommand.lua")
local SelectCommand = require("Commands/SelectCommand.lua")
local EnableInputCommand = require("Commands/EnableInputCommand.lua") 

local ChessBoardCommandsDebugMenu = { }
ChessBoardCommandsDebugMenu.fromMovePosition = Vector2Int.new(0, 0)
ChessBoardCommandsDebugMenu.toMovePosition = Vector2Int.new(1, 1)
ChessBoardCommandsDebugMenu.fromAttackPosition = Vector2Int.new(0, 0)
ChessBoardCommandsDebugMenu.toAttackPosition = Vector2Int.new(1, 1)
ChessBoardCommandsDebugMenu.selectPosition = Vector2Int.new(0, 0)

function ChessBoardCommandsDebugMenu:onDrawMenu()
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

return ChessBoardCommandsDebugMenu