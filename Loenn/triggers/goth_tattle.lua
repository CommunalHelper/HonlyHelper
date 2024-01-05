local gothTattle = {}

gothTattle.name = "HonlyHelper/GothTattle"

gothTattle.fieldInformation = {
    DialogAmount = {
        fieldType = "integer",
        minimumValue = 1
    }
}

gothTattle.placements = {
    name = "talk_to_badeline",
    alternativeName = {"goth_tattle"},
    data = {
        GothDialogID = "GothID",
        DialogAmount = 1,
        Loops = false,
        Ends = false
    }
}

return gothTattle
