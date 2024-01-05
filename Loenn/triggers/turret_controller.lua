local turretController = {}

local turretActions = {
    "HeliFadeIn", "HeliOn", "HeliLeave", "GunOnlyOn", "GunOnlyOff"
}

turretController.name = "HonlyHelper/TurretController"
turretController.fieldInformation = {
    turretAction = {
        options = turretActions,
        editable = false
    }
}
turretController.placements = {
    name = "turret_controller",
    data = {
        turretID = "TurretID",
        turretAction = "HeliFadeIn"
    }
}

return turretController
