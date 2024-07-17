local enums = require("consts.celeste_enums")
local environmentalSounds = table.keys(enums.environmental_sounds)

table.sort(environmentalSounds)

local flagSoundSource = {}

flagSoundSource.name = "HonlyHelper/FlagSoundSource"
flagSoundSource.depth = 0
flagSoundSource.texture = "@Internal@/sound_source"
flagSoundSource.fieldInformation = {
    sound = {
        options = environmentalSounds
    }
}
flagSoundSource.placements = {
    name = "flag_sound_source",
    data = {
        sound = "sound",
        flag = "flag",
        AllowFade = false
    }
}

return flagSoundSource
