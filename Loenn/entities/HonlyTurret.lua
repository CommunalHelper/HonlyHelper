local default = {}

default.name = "HonlyHelper/Turret"
default.depth = 0
default.texture = "objects/HonlyHelper/Turret/idle00"
default.placements = {
    name = "default",
    data = {
        accelerationMultiplier = 1.0,
        cooldownTime = 2.0,
        aimTime = 2.0,
        desiredBulletSpeed = 2000.0,
        randomCooldownMultiplier = 0.0,
        turretID = "TurretID"
    }
}

return default