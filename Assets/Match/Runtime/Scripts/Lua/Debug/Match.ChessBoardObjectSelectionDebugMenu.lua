local ChessBoardObjectSelectionDebugMenu = { }

function ChessBoardObjectSelectionDebugMenu:onDrawMenu()
    GUILayout.label("Has Selected Object: " .. tostring(ChessBoardObjectSelection.hasSelectedObject))
end

return ChessBoardObjectSelectionDebugMenu