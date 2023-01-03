local default = {}

default.name = "HonlyHelper/TurretController"
default.placements = {
    name = "default",
    data = {
        turretID = "TurretID",
        turretAction = "HeliFadeIn"
    }
}

default.fieldInformation = {
    turretAction = {
        options = {
            "HeliFadeIn",
            "HeliOn",
            "HeliLeave",
            "GunOnlyOn",
            "GunOnlyOff"
        },
        editable = false
    }
}

return default