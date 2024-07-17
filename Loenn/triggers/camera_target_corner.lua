local cameraTargetCorner = {}

local corners = {
    "BottomLeft", "BottomRight", "TopLeft", "TopRight"
}

cameraTargetCorner.name = "HonlyHelper/CameraTargetCornerTrigger"
cameraTargetCorner.nodeLimits = {1, 1}
cameraTargetCorner.fieldInformation = {
    positionMode = {
        options = corners,
        editable = false
    }
}
cameraTargetCorner.placements = {
    name = "camera_target_corner",
    data = {
        lerpStrength = 0.0,
        positionMode = "BottomLeft",
        xOnly = false,
        yOnly = false,
        deleteFlag = ""
    }
}

return cameraTargetCorner
