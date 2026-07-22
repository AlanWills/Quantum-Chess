local MoveCommand = require("Commands/MoveCommand.lua")
local AttackCommand = require("Commands/AttackCommand.lua")
local SelectCommand = require("Commands/SelectCommand.lua")
local EnableInputCommand = require("Commands/EnableInputCommand.lua") 
local EndTurnCommand = require("Commands/EndTurnCommand.lua")
local AddBoardObjectCommand = require("Commands/AddBoardObjectCommand.lua")
local RemoveBoardObjectCommand = require("Commands/RemoveBoardObjectCommand.lua")
local ClearBoardCommand = require("Commands/ClearBoardCommand.lua")

local MatchCommandsDebugMenu = { }
MatchCommandsDebugMenu.fromMovePosition = Vector2Int.new(0, 0)
MatchCommandsDebugMenu.toMovePosition = Vector2Int.new(1, 1)
MatchCommandsDebugMenu.fromAttackPosition = Vector2Int.new(0, 0)
MatchCommandsDebugMenu.toAttackPosition = Vector2Int.new(1, 1)
MatchCommandsDebugMenu.selectPosition = Vector2Int.new(0, 0)
MatchCommandsDebugMenu.addBoardObjectGuid = 0
MatchCommandsDebugMenu.addBoardObjectPosition = Vector2Int.new(0, 0)
MatchCommandsDebugMenu.removeBoardObjectPosition = Vector2Int.new(0, 0)

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

    GUILayout.beginSection("End Turn")
        if GUILayout.button("End Turn") then
            EndTurnCommand.new()
        end
    GUILayout.endSection()
    
    GUILayout.beginSection("Add Pieces")
        self.addBoardObjectGuid = GUILayout.intField("Guid", self.addBoardObjectGuid)
        self.addBoardObjectPosition = GUILayout.vector2IntField("Position", self.addBoardObjectPosition)
        
        self:drawAddPieces("Pawn")
        self:drawAddPieces("Knight")
        self:drawAddPieces("Bishop")
        self:drawAddPieces("Rook")
        self:drawAddPieces("Queen")
        self:drawAddPieces("King")
    GUILayout.endSection()
    
    GUILayout.beginSection("Remove Board Objects")
        self.removeBoardObjectPosition = GUILayout.vector2IntField("Position", self.removeBoardObjectPosition)
        
        GUILayout.beginHorizontal()
            if GUILayout.button("Remove") then
                RemoveBoardObjectCommand.at(self.removeBoardObjectPosition)
            end
        GUILayout.endHorizontal()
    GUILayout.endSection()
    
    GUILayout.beginSection("Board")
        GUILayout.beginHorizontal()
            if GUILayout.button("Clear") then
                ClearBoardCommand.new()
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
            AddBoardObjectCommand.presetChessPiece(self.addBoardObjectGuid, { colour = "White", type = type, position = self.addBoardObjectPosition })
        end
    
        if GUILayout.button("Black " .. type) then
            AddChessPieceCommand.presetChessPiece(self.addBoardObjectGuid, { colour = "Black", type = type, position = self.addBoardObjectPosition })
        end
    GUILayout.endHorizontal()
end

return MatchCommandsDebugMenu