local enums = require("consts.celeste_enums")

local cameraTargetCrossfade = {}

cameraTargetCrossfade.name = "HonlyHelper/CameraTargetCrossfadeTrigger"
cameraTargetCrossfade.nodeLimits = {2, 2}
cameraTargetCrossfade.nodeLineRenderType = "fan"
cameraTargetCrossfade.fieldInformation = {
    positionMode = {
        options = enums.trigger_position_modes,
        editable = false
    }
}
cameraTargetCrossfade.placements = {
    name = "camera_target_crossfade",
    data = {
        lerpStrengthA = 0.0,
        lerpStrengthB = 0.0,
        positionMode = "NoEffect",
        xOnly = false,
        yOnly = false,
        deleteFlag = ""
    }
}

return cameraTargetCrossfade
