local AddBoardObjectCommand = require("Commands/AddBoardObjectCommand.lua")

local CampaignFactory = { }

function CampaignFactory.loadPlayerCurrentUnits(deploymentPositions)
    for i, guid in ipairs(CampaignRecord:getCurrentUnits()) do
        AddBoardObjectCommand.presetPiece(guid, { position = deploymentPositions[i] })
    end
end

return CampaignFactory