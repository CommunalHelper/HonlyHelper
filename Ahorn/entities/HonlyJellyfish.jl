module HonlyJellyfishModule

using ..Ahorn, Maple

@mapdef Entity "HonlyHelper/Jellyfish" HonlyJellyfish(x::Integer, y::Integer)

sprite = "objects/HonlyHelper/Creechure/Jellyfish/jelly.png"

function Ahorn.selection(entity::HonlyJellyfish)
    x, y = Ahorn.position(entity)
    return Ahorn.Rectangle(x-4, y-4, 8, 8)
end

Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::HonlyJellyfish, room::Maple.Room) = Ahorn.drawSprite(ctx, sprite, 0, 0)

end