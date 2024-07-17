local fakeTilesHelper = require("helpers.fake_tiles")

local risingBlock = {}

risingBlock.name = "HonlyHelper/RisingBlock"

function risingBlock.placements()
    return {
        name = "rising_block",
        data = {
            tiletype = "3",
            width = 8,
            height = 8
        }
    }
end

risingBlock.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype", false)
risingBlock.fieldInformation = fakeTilesHelper.getFieldInformation("tiletype")

return risingBlock
