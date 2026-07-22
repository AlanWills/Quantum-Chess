local BoardObjectSelectionDebugMenu = { }

function BoardObjectSelectionDebugMenu:onDrawMenu()
    GUILayout.label("Has Selected Object: " .. tostring(BoardObjectSelection.hasSelectedObject))
end

return BoardObjectSelectionDebugMenu