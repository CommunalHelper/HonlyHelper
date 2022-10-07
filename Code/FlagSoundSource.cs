using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.HonlyHelper {
    [Tracked]
    [CustomEntity("HonlyHelper/FlagSoundSource")]
    public class FlagSoundSourceEntity : Entity {
        private readonly string eventName;
        private readonly string flagName;
        private bool playing = false;
        private readonly bool fademode;
        private readonly SoundSource sfx;

        public FlagSoundSourceEntity(EntityData data, Vector2 offset)
            : base(data.Position + offset) {
            fademode = data.Bool("AllowFade");
            flagName = data.Attr("flag");
            Tag = Tags.TransitionUpdate;
            Add(sfx = new SoundSource());
            eventName = SFX.EventnameByHandle(data.Attr("sound"));
            Visible = true;
            Depth = -8500;
        }

        public override void Update() {
            base.Update();
            if (!SceneAs<Level>().Session.GetFlag(flagName) && playing) {
                sfx.Stop(fademode);
                playing = false;
            } else if (SceneAs<Level>().Session.GetFlag(flagName) && !playing) {
                sfx.Play(eventName);
                playing = true;
            }
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            if (SceneAs<Level>().Session.GetFlag(flagName)) {
                sfx.Play(eventName);
                playing = true;
            }
        }
    }
}
