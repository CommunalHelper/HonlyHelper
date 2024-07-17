local fakeTilesHelper = require("helpers.fake_tiles")
local utils = require("utils")

local floatyBgTile = {}

floatyBgTile.name = "HonlyHelper/FloatyBgTile"
floatyBgTile.depth = 10000
function floatyBgTile.placements()
    return {
        name = "floaty_bg_tile",
        data = {
            tiletype = "3",
            disableSpawnOffset = false,
            width = 8,
            height = 8
        }
    }
end

floatyBgTile.fieldInformation = fakeTilesHelper.getFieldInformation("tiletype", "tilesBg")

-- Filter by floaty bg tiles sharing the same tiletype
local function getSearchPredicate(entity)
    return function(target)
        return entity._name == target._name and entity.tiletype == target.tiletype
    end
end

function floatyBgTile.sprite(room, entity)
    local relevantBlocks = utils.filter(getSearchPredicate(entity), room.entities)
    local firstEntity = relevantBlocks[1] == entity

    if firstEntity then
        -- Can use simple render, nothing to merge together
        if #relevantBlocks == 1 then
            return fakeTilesHelper.getEntitySpriteFunction("tiletype", false, "tilesBg", {1.0, 1.0, 1.0, 0.4})(room, entity)
        end

        return fakeTilesHelper.getCombinedEntitySpriteFunction(relevantBlocks, "tiletype", false, "tilesBg", {1.0, 1.0, 1.0, 0.4})(room)
    end

    local entityInRoom = utils.contains(entity, relevantBlocks)

    -- Entity is from a placement preview
    if not entityInRoom then
        return fakeTilesHelper.getEntitySpriteFunction("tiletype", false, "tilesBg", {1.0, 1.0, 1.0, 0.4})(room, entity)
    end
end

return floatyBgTile
