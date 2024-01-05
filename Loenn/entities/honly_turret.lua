local drawableSprite = require("structs.drawable_sprite")

local turret = {}

turret.name = "HonlyHelper/Turret"
turret.fieldInformation = {
    aimTime = {
        fieldType = "number",
        minimumValue = 0.0
    },
    cooldownTime = {
        fieldType = "number",
        minimumValue = 0.0
    },
    randomCooldownMultiplier = {
        fieldType = "number",
        minimumValue = 0.0
    },
    accelerationMultiplier = {
        fieldType = "number",
        minimumValue = 0.0
    }
}
turret.placements = {
    name = "turret",
    data = {
        turretID = "TurretID",
        aimTime = 2.0,
        cooldownTime = 2.0,
        randomCooldownMultiplier = 0.0,
        desiredBulletSpeed = 2000.0,
        accelerationMultiplier = 1.0
    }
}

local offsetY = -4
local texture = "objects/HonlyHelper/Turret/idle00"

function turret.sprite(room, entity)
    local sprite = drawableSprite.fromTexture(texture, entity)

    sprite.y += offsetY

    return sprite
end

return turret
