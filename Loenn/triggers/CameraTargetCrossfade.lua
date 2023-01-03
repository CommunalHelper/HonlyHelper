local enums = require("consts.celeste_enums")

local default = {}

default.name = "HonlyHelper/CameraTargetCrossfadeTrigger"
default.placements = {
    name = "default",
    data = {
        deleteFlag = "",
        lerpStrengthA = 0.0,
        lerpStrengthB = 0.0,
        positionMode = "NoEffect",
        xOnly = false,
        yOnly = false
    }
}

default.nodeLimits = {2, 2}
default.nodeVisibility = "selected"
default.nodeLineRenderType = "fan"

default.fieldInformation = {
    positionMode = {
        options = enums.trigger_position_modes,
        editable = false
    }
}

return default