using Celeste.Mod.Entities;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.HonlyHelper {
    [Pooled]
    [Tracked]
    [CustomEntity("HonlyHelper/TurretBullet")]
    public class TurretBullet : Entity {
        private readonly ParticleType particleType3 = new(FallingBlock.P_LandDust);
        private Turret turret;
        private Level level;
        private Vector2 speed;
        private Vector2 anchor;
        private Player target;
        private float aimAngle;
        private bool dead;
        private bool DestinyBound = false;
        private readonly Sprite sprite;
        private float bulletSpeed;
        private EventInstance whistleInstance;
        private Vector2 tempbetween;
        private readonly int tracerlength = 3;
        private Vector2[] posbuffer;

        public TurretBullet()
            : base(Vector2.Zero) {
            sprite = new Sprite(GFX.Game, "objects/HonlyHelper/Turret/Bullet/");
            sprite.AddLoop("Idle", "bullet", 1f, 0);
            Collider = new Hitbox(2f, 2f, -1f, -1f);
            Add(new PlayerCollider(OnPlayer));
            Depth = -1000000;
        }

        public TurretBullet Init(Turret turret, Player target, float aimAngle, float bulletSpeed) {
            this.turret = turret;
            anchor = Position = turret.Center;
            this.target = target;
            this.aimAngle = aimAngle;
            this.bulletSpeed = bulletSpeed;
            posbuffer = new Vector2[tracerlength];
            dead = false;
            DestinyBound = false;

            for (int i = 0; i < tracerlength; i++) {
                posbuffer[i] = turret.Center;
            }

            InitSpeed();
            return this;
        }

        private void InitSpeed() {
            speed = target != null ? Vector2.UnitX * bulletSpeed : Vector2.UnitX * bulletSpeed;
            if (aimAngle != 0f) {
                speed = speed.Rotate(aimAngle);
            }
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            level = SceneAs<Level>();
            sprite.Position = (-24 * Vector2.UnitX) - (25 * Vector2.UnitY);
            Add(sprite);
            sprite.Play("Idle");
            whistleInstance = Audio.Play("event:/HonlyHelper/bullet_whistle", Center);
        }

        public override void Removed(Scene scene) {
            base.Removed(scene);
            level = null;
        }

        public override void Update() {
            base.Update();
            if (DestinyBound) {
                Destroy();
            }

            anchor += speed * Engine.DeltaTime;
            if (Scene.CollideCheck<Player>(anchor, Position)) {
                Vector2 tempanchor = anchor;
                Vector2 tempposition = Position;
                tempbetween = (anchor + Position) / 2;

                for (int i = 0; i < 6; i++) {
                    if (Scene.CollideCheck<Player>(tempbetween, tempposition)) {
                        tempanchor = tempbetween;
                    } else {
                        tempposition = tempbetween;
                    }

                    tempbetween = (tempanchor + tempposition) / 2;
                }

                OnPlayer(target);
                Position = turret.Center;
                DestinyBound = true;
                UpdateRenderBuffer(true);
            } else if (Scene.CollideCheck<Solid>(anchor, Position)) {
                Vector2 tempanchor = anchor;
                Vector2 tempposition = Position;
                tempbetween = (anchor + Position) / 2;

                for (int i = 0; i < 6; i++) {
                    if (Scene.CollideCheck<Solid>(tempbetween, tempposition)) {
                        tempanchor = tempbetween;
                    } else {
                        tempposition = tempbetween;
                    }

                    tempbetween = (tempanchor + tempposition) / 2;
                }

                level.Particles.Emit(particleType3, 4, tempbetween, Vector2.One * 1f, (-speed).Angle());
                Audio.Play("event:/game/04_cliffside/snowball_impact", tempbetween);

                FallingBlock fallingBlock = CollideFirst<FallingBlock>(tempbetween);
                if (fallingBlock != null) {
                    fallingBlock.Triggered = true;
                }

                DashBlock dashBlock = CollideFirst<DashBlock>(tempbetween);
                if (dashBlock != null) {
                    dashBlock.Break(tempbetween, speed, true, true);
                }

                DestinyBound = true;
                Position = turret.Center;
                UpdateRenderBuffer(true);
            } else {
                //makes sure bullets aren't suicidal
                DestinyBound = false;

                UpdateRenderBuffer(false);
                Position = anchor;
                Level level = SceneAs<Level>();

                // should delete every bullet that goes out of bounds
                if (!level.IsInBounds(this)) {
                    Destroy();
                }
            }

            Audio.Position(whistleInstance, Position);
        }

        private void UpdateRenderBuffer(bool collisionDetected) {
            for (int i = tracerlength - 1; i > 0; i--) {
                posbuffer[i] = posbuffer[i - 1];
            }

            posbuffer[0] = collisionDetected ? tempbetween : anchor;
        }

        public override void Render() {
            for (int i = 0; i < tracerlength - 1; i++) {
                Draw.Line(posbuffer[i], posbuffer[i + 1], Color.White);
            }

            base.Render();
        }

        public void Destroy() {
            DestinyBound = false;
            Audio.Stop(whistleInstance, false);
            dead = true;
            RemoveSelf();
        }

        private void OnPlayer(Player player) {
            if (!dead) {
                player.Die((player.Center - Position).SafeNormalize());
            }
        }
    }
}
