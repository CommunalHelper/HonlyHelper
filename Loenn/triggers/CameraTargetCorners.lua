local default = {}

default.name = "HonlyHelper/CameraTargetCornerTrigger"
default.placements = {
    name = "default",
    data = {
        deleteFlag = "",
        lerpStrength = 0.0,
        positionMode = "BottomLeft",
        xOnly = false,
        yOnly = false
    }
}

default.nodeLimits = {1, 1}
default.nodeVisibility = "selected"
default.nodeLineRenderType = "line"

default.fieldInformation = {
    positionMode = {
        options = {
            "BottomLeft",
            "BottomRight",
            "TopLeft",
            "TopRight"
        },
        editable = false
    }
}

return default