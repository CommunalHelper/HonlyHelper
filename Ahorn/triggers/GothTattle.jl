module HonlyHelperGothTattle
using ..Ahorn, Maple

@mapdef Trigger "HonlyHelper/GothTattle" GothTattle(x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight, GothDialogID::String="GothID", DialogAmount::Integer=1,Loops::Bool=false,Ends::Bool=false)

const placements = Ahorn.PlacementDict(
    "Badeline Tattle Zone (HonlyHelper)" => Ahorn.EntityPlacement(
        GothTattle,
        "rectangle",
    ),
)

end