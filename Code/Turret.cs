using Celeste.Mod.Entities;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.HonlyHelper {
    [Tracked]
    [CustomEntity("HonlyHelper/Turret")]
    public class Turret : Entity {
        public StateMachine StateMachine;

        private readonly Sprite TurretSprite;

        public float DelayTimer;

        public float angle;

        private readonly Vector2[] sample = new Vector2[5];
        private readonly bool[] samplegot = { false, false, false, false };

        private readonly float desiredBulletSpeed;
        private readonly float randomDelay;
        private readonly float cooldownTime;
        private readonly float aimTime;
        private readonly float accelerationMultiplier;

        private bool activated;
        private bool countbool;
        private bool countbool2;
        public string turretID;

        private readonly int seed;

        private EventInstance helicopterInstance;

        public Turret(Vector2 position, float desiredBulletSpeed, float randomDelay, string turretID, float cooldownTime, float aimTime, float accelerationMultipler)
            : base(position) {
            StateMachine = new StateMachine(3);
            StateMachine.SetCallbacks(0, IdleUpdate);
            StateMachine.SetCallbacks(1, DelayUpdate, null, DelayBegin);
            StateMachine.SetCallbacks(2, CooldownUpdate, null, CooldownBegin);
            Add(StateMachine);
            StateMachine.State = 0;
            activated = false;
            countbool = false;

            this.cooldownTime = cooldownTime;
            this.aimTime = aimTime;
            this.turretID = turretID;
            this.desiredBulletSpeed = desiredBulletSpeed;
            this.randomDelay = randomDelay;
            accelerationMultiplier = accelerationMultipler;

            // sprites n stuff later or maybe not
            TurretSprite = new Sprite(GFX.Game, "objects/HonlyHelper/Turret/");

            // fix sprites later?
            TurretSprite.AddLoop("Idle", "idle", 0.15f, 0);
            TurretSprite.AddLoop("Delay", "delay", 0.15f, 0);
            TurretSprite.AddLoop("Cooldown", "cooldown", 0.15f, 0);
            Add(TurretSprite);

            seed = 0;
            int inc = 1;
            foreach (char c in turretID) {
                seed += inc * short.Parse(Convert.ToString(c, 10));
                inc++;
            }

            //hitbox if anyone needs it
            //Collider = new Hitbox(24f, 24f, -16f, -16f);
        }

        public Turret(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Float("desiredBulletSpeed", 1.0f), data.Float("randomCooldownMultiplier", 0f), data.Attr("turretID"), data.Float("cooldownTime"), data.Float("aimTime"), data.Float("accelerationMultiplier")) {
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            TurretSprite.Position = (-16 * Vector2.UnitX) - (16 * Vector2.UnitY);
            TurretSprite.Play("Idle");
        }

        public override void Removed(Scene scene) {
            base.Removed(scene);
            if (helicopterInstance != null) {
                Audio.Stop(helicopterInstance, true);
            }

            activated = false;
            countbool = false;
            countbool2 = false;
        }

        public void Helicopter_FadeIn() {
            if (!activated && !countbool && !countbool2) {
                helicopterInstance = Audio.Play("event:/HonlyHelper/helicopter", Center);
                Audio.SetParameter(helicopterInstance, "Helicopter_Fade", 0f);
                Audio.SetParameter(helicopterInstance, "Helicopter_Mute", 0f);
                countbool = true;
                DelayTimer = 10f;
            }
        }

        public void Helicopter_On() {
            if (!activated && !countbool && !countbool2) {
                helicopterInstance = Audio.Play("event:/HonlyHelper/helicopter", Center);
                Audio.SetParameter(helicopterInstance, "Helicopter_Fade", 1f);
                Audio.SetParameter(helicopterInstance, "Helicopter_Mute", 1f);
                activated = true;
            }
        }

        public void Gun_Only_On() {
            activated = true;
            countbool = false;
            countbool2 = false;
        }

        public void Gun_Only_Off() {
            activated = false;
            countbool = false;
            countbool2 = false;
        }

        public void Helicopter_Leave() {
            if (helicopterInstance != null) {
                Audio.SetParameter(helicopterInstance, "Helicopter_Fade", 0f);
                Audio.Stop(helicopterInstance, true);
            }

            activated = false;
        }

        private bool CanSeePlayer(Player player) {
            if (player == null) {
                return false;
            }

            // less strict method, it should do like a cube, 2x2 big
            return (!Scene.CollideCheck<Solid>(Center + Vector2.One, player.Center - Vector2.UnitY + Vector2.UnitX) || !Scene.CollideCheck<Solid>(Center + Vector2.One, player.Center - Vector2.UnitY - Vector2.UnitX))
                && (!Scene.CollideCheck<Solid>(Center + Vector2.One, player.Center - Vector2.UnitY + Vector2.UnitX) || !Scene.CollideCheck<Solid>(Center + Vector2.One, player.Center - Vector2.UnitY - Vector2.UnitX));
        }

        private void Shoot(Player player, float angle, float bulletSpeed) {
            SceneAs<Level>().Add(Engine.Pooler.Create<TurretBullet>().Init(this, player, angle, bulletSpeed));
        }

        private void SoundUpdate() {
            if (helicopterInstance != null) {
                Audio.Position(helicopterInstance, Center);
            }
        }

        public int IdleUpdate() {
            SoundUpdate();
            Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
            if (!activated) {
                if (countbool) {
                    DelayTimer -= Engine.DeltaTime;

                    if (DelayTimer <= 0f) {
                        countbool = false;
                        countbool2 = true;
                        DelayTimer = 10f;
                        Audio.SetParameter(helicopterInstance, "Helicopter_Fade", 1f);
                        Audio.SetParameter(helicopterInstance, "Helicopter_Mute", 1f);
                    }
                } else if (countbool2) {
                    DelayTimer -= Engine.DeltaTime;

                    if (DelayTimer <= 0f) {
                        countbool2 = false;
                        activated = true;
                        if (CanSeePlayer(player)) {
                            return 1;
                        }
                    }
                }

                return 0;
            } else if (CanSeePlayer(player)) {

                return 1;
            }

            return 0;
        }

        public int DelayUpdate() {
            SoundUpdate();
            Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
            if (player == null || !activated) {
                TurretSprite.Play("Idle");
                return 0;
            }

            if (DelayTimer >= 0f) {
                DelayTimer -= Engine.DeltaTime;

                if (DelayTimer <= 0f) {

                    return 2;
                } else if (DelayTimer <= aimTime / 4 && !samplegot[3]) {
                    sample[3] = player.Center;
                    samplegot[3] = true;

                } else if (DelayTimer <= aimTime / 2 && !samplegot[2]) {
                    sample[2] = player.Center;
                    samplegot[2] = true;
                } else if (DelayTimer <= 0.75 * aimTime && !samplegot[1]) {
                    sample[1] = player.Center;
                    samplegot[1] = true;

                } else if (DelayTimer <= aimTime && !samplegot[0]) {
                    sample[0] = player.Center;
                    samplegot[0] = true;
                }
            }

            if (CanSeePlayer(player)) {
                return 1;
            }

            TurretSprite.Play("Idle");
            return 0;
        }

        public void DelayBegin() {
            TurretSprite.Play("Delay");
            DelayTimer = aimTime;
            int i = 0;
            foreach (bool j in samplegot) {
                samplegot[i] = false;
                i++;
            }
        }

        public int CooldownUpdate() {
            SoundUpdate();
            if (!activated) {
                TurretSprite.Play("Idle");
                return 0;
            }

            if (DelayTimer >= 0f) {
                DelayTimer -= Engine.DeltaTime;
                if (DelayTimer <= 0f) {
                    TurretSprite.Play("Idle");
                    return 0;
                }
            }

            return 2;
        }
        public void CooldownBegin() {
            TurretSprite.Play("Cooldown");
            Random rand = new(seed);
            DelayTimer = cooldownTime * (1 + ((float)rand.NextDouble() * randomDelay));

            //big fucking jank stupid PoS solution, apparently BiS??
            Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
            sample[4] = player.Center;
            Vector2 speed1 = sample[1] - sample[0];
            Vector2 speed2 = sample[2] - sample[1];
            Vector2 speed3 = sample[3] - sample[2];
            Vector2 speed4 = sample[4] - sample[3];

            Vector2 accel1 = speed2 - speed1;
            Vector2 accel2 = speed3 - speed2;
            Vector2 accel3 = speed4 - speed3;
            Vector2 accelavg = (0.4f * accel3) + (0.3f * accel2) + (0.3f * accel1);
            Vector2 speed5 = speed4 + (accelerationMultiplier * accelavg);

            float timetohit = (player.Center - Center).Length() / desiredBulletSpeed;
            Vector2 playerExpect = player.Center + (speed5 * (4 / aimTime) * timetohit);
            float bulletSpeed = (playerExpect - Center).Length() * (1 / timetohit);

            angle = (playerExpect - Center).Angle();
            // also shoot or something
            Shoot(player, angle, bulletSpeed);
            Audio.Play("event:/HonlyHelper/shot", Center);
        }
    }
}
