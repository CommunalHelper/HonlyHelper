local drawableSprite = require("structs.drawable_sprite")

local pettableCat = {}

pettableCat.name = "HonlyHelper/PettableCat"
pettableCat.depth = 1000
pettableCat.placements = {
    name = "pettable_cat",
    data = {
        catFlag = "CatHasBeenPet"
    }
}

local offsetY = 4
local texture = "characters/HonlyHelper/pettableCat/spoons_idle00"

function pettableCat.sprite(room, entity)
    local sprite = drawableSprite.fromTexture(texture, entity)

    sprite.y += offsetY

    return sprite
end

return pettableCat
