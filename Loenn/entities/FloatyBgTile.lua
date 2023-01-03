local fakeTilesHelper = require("helpers.fake_tiles")

local default = {}

default.name = "HonlyHelper/FloatyBgTile"
default.depth = 0
default.placements = {
    name = "default",
    data = {
        width = 8,
        height = 8,
        tiletype = "3",
        disableSpawnOffset = false
    }
}

default.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype", true, "tilesBg", {1.0, 1.0, 1.0, 0.7})
default.fieldInformation = fakeTilesHelper.getFieldInformation("tiletype")

return default