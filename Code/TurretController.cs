using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.HonlyHelper {
    [Tracked]
    [CustomEntity("HonlyHelper/TurretController")]
    public class TurretController : Trigger {
        private readonly string turretID;
        private readonly string turretActionRead;
        private Turret turret;

        public TurretController(EntityData data, Vector2 offset)
            : base(data, offset) {
            turretID = data.Attr("turretID");
            turretActionRead = data.Attr("turretAction");
        }

        public override void Added(Scene scene) {
            base.Added(scene);

            foreach (Turret t in SceneAs<Level>().Tracker.GetEntities<Turret>()) {
                if (t.TurretID == turretID) {
                    turret = t;
                    break;
                }
            }
        }

        public override void OnEnter(Player player) {
            base.OnEnter(player);
            switch (turretActionRead) {
                case "HeliFadeIn":
                    turret.Helicopter_FadeIn();
                    break;
                case "HeliOn":
                    turret.Helicopter_On();
                    break;
                case "HeliLeave":
                    turret.Helicopter_Leave();
                    break;
                case "GunOnlyOn":
                    turret.Gun_Only_On();
                    break;
                case "GunOnlyOff":
                    turret.Gun_Only_Off();
                    break;
                default:
                    break;
            }
        }
    }
}
