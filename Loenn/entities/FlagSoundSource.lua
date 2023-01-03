local enums = require("consts.celeste_enums")

local default = {}

default.name = "HonlyHelper/FlagSoundSource"
default.texture = "@Internal@/sound_source"
default.depth = 0
default.placements = {
    name = "default",
    data = {
        sound = "env_loc_10_kevinpc",
        allowFade = false,
        flag = "flag"
    }
}

default.fieldInformation = {
    track = {
        options = enums.environmental_sounds,
        editable = true
    }
}

return default