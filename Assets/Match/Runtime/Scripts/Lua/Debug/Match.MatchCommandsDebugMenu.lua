local MoveCommand = require("Commands/MoveCommand.lua")
local AttackCommand = require("Commands/AttackCommand.lua")
local SelectCommand = require("Commands/SelectCommand.lua")
local EnableInputCommand = require("Commands/EnableInputCommand.lua") 
local SetActiveColourCommand = require("Commands/SetActiveColourCommand.lua")
local AddChessPieceCommand = require("Commands/AddChessPieceCommand.lua")
local RemoveChessPieceCommand = require("Commands/RemoveChessPieceCommand.lua")
local ClearChessBoardCommand = require("Commands/ClearChessBoardCommand")

local MatchCommandsDebugMenu = { }
MatchCommandsDebugMenu.fromMovePosition = Vector2Int.new(0, 0)
MatchCommandsDebugMenu.toMovePosition = Vector2Int.new(1, 1)
MatchCommandsDebugMenu.fromAttackPosition = Vector2Int.new(0, 0)
MatchCommandsDebugMenu.toAttackPosition = Vector2Int.new(1, 1)
MatchCommandsDebugMenu.selectPosition = Vector2Int.new(0, 0)
MatchCommandsDebugMenu.addPieceGuid = 0
MatchCommandsDebugMenu.addPiecePosition = Vector2Int.new(0, 0)
MatchCommandsDebugMenu.removePiecePosition = Vector2Int.new(0, 0)

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
    
    GUILayout.beginSection("Add Pieces")
        self.addPieceGuid = GUILayout.intField("Guid", self.addPieceGuid)
        self.addPiecePosition = GUILayout.vector2IntField("Position", self.addPiecePosition)
        
        self:drawAddPieces("Pawn")
        self:drawAddPieces("Knight")
        self:drawAddPieces("Bishop")
        self:drawAddPieces("Rook")
        self:drawAddPieces("Queen")
        self:drawAddPieces("King")
    GUILayout.endSection()
    
    GUILayout.beginSection("Remove Pieces")
        self.removePiecePosition = GUILayout.vector2IntField("Position", self.removePiecePosition)
        
        GUILayout.beginHorizontal()
            if GUILayout.button("Remove") then
                RemoveChessPieceCommand.at(self.removePiecePosition)
            end
        GUILayout.endHorizontal()
    GUILayout.endSection()
    
    GUILayout.beginSection("Board")
        GUILayout.beginHorizontal()
            if GUILayout.button("Clear") then
                ClearChessBoardCommand.new()
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

function MatchCommandsDebugMenu:drawAddPieces(type)
    GUILayout.beginHorizontal()
        if GUILayout.button("White " .. type) then
            AddChessPieceCommand.new(self.addPieceGuid, { colour = "White", type = type, position = self.addPiecePosition })
        end
    
        if GUILayout.button("Black " .. type) then
            AddChessPieceCommand.new(self.addPieceGuid, { colour = "Black", type = type, position = self.addPiecePosition })
        end
    GUILayout.endHorizontal()
end

return MatchCommandsDebugMenu