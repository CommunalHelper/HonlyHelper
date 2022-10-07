using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.HonlyHelper {
    [Tracked]
    [CustomEntity(
        "HonlyHelper/BouncySpikesUp = BounceUp",
        "HonlyHelper/BouncySpikesDown = BounceDown",
        "HonlyHelper/BouncySpikesLeft = BounceLeft",
        "HonlyHelper/BouncySpikesRight = BounceRight"
    )]
    public class BouncySpikes : Entity {
        public static Entity BounceUp(Level level, LevelData levelData, Vector2 position, EntityData entityData) {
            return new BouncySpikes(entityData, position, Directions.Up);
        }

        public static Entity BounceDown(Level level, LevelData levelData, Vector2 position, EntityData entityData) {
            return new BouncySpikes(entityData, position, Directions.Down);
        }

        public static Entity BounceLeft(Level level, LevelData levelData, Vector2 position, EntityData entityData) {
            return new BouncySpikes(entityData, position, Directions.Left);
        }

        public static Entity BounceRight(Level level, LevelData levelData, Vector2 position, EntityData entityData) {
            return new BouncySpikes(entityData, position, Directions.Right);
        }

        public enum Directions {
            Up,
            Down,
            Left,
            Right
        }

        public Directions Direction;

        private Vector2 speed;

        private readonly string texture;
        private Image image;

        private readonly int size;
        private bool dashedintoit;
        private bool intoit;
        private readonly bool FreezeFrameEnable = false;

        private readonly ParticleType bounceParticle = new(Player.P_DashA);
        private readonly float particleAngle;
        private Vector2 particlePosAdjust;
        private Vector2 particlePosAdjustTwo;

        public BouncySpikes(Vector2 position, int size, Directions direction, string texture, bool FreezeFrameEnable)
            : base(position) {
            Depth = -1;
            Direction = direction;
            this.size = size;
            this.texture = texture;
            this.FreezeFrameEnable = FreezeFrameEnable;

            // making the bounce particles
            bounceParticle.Color = Color.LightBlue;
            bounceParticle.Color2 = Color.LightBlue;
            bounceParticle.FadeMode = ParticleType.FadeModes.Late;
            bounceParticle.SpeedMin = 20f;
            bounceParticle.SpeedMax = 25f;
            bounceParticle.LifeMin = 0.5f;
            bounceParticle.LifeMax = 0.7f;

            switch (Direction) {
                case Directions.Up:
                    particleAngle = (float)(11 * Math.PI / 8);
                    particlePosAdjust = Vector2.Zero;
                    particlePosAdjustTwo = -Vector2.UnitX;
                    break;
                case Directions.Down:
                    particleAngle = (float)(3 * Math.PI / 8);
                    particlePosAdjust = new Vector2(0, -10);
                    particlePosAdjustTwo = Vector2.UnitX;
                    break;
                case Directions.Left:
                    particleAngle = (float)(7 * Math.PI / 8);
                    particlePosAdjust = new Vector2(5, -8);
                    particlePosAdjustTwo = Vector2.UnitY;
                    break;
                case Directions.Right:
                    particleAngle = (float)(15 * Math.PI / 8);
                    particlePosAdjust = new Vector2(-4, -8);
                    particlePosAdjustTwo = -Vector2.UnitY;
                    break;
            }

            switch (direction) {
                case Directions.Up:
                    Collider = new Hitbox(size, 8f, 0f, -8f);
                    Add(new LedgeBlocker());
                    break;
                case Directions.Down:
                    Collider = new Hitbox(size, 8f);
                    break;
                case Directions.Left:
                    Collider = new Hitbox(8f, size, -8f);
                    Add(new LedgeBlocker());
                    break;
                case Directions.Right:
                    Collider = new Hitbox(8f, size);
                    Add(new LedgeBlocker());
                    break;
            }

            Add(new PlayerCollider(OnCollide));
            Add(new HoldableCollider(OnHoldableCollide));
            Add(new StaticMover {
                OnShake = OnShake,
                SolidChecker = IsRiding,
                JumpThruChecker = IsRiding
            });
        }

        public BouncySpikes(EntityData data, Vector2 offset, Directions dir)
            : this(data.Position + offset, GetSize(data, dir), dir, data.Attr("texture", "objects/HonlyHelper/BouncySpikes/bouncer"), data.Bool("FreezeFrameEnable")) {
        }

        public override void Added(Scene scene) {
            base.Added(scene);

            string str = Direction.ToString().ToLower();

            List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures(texture + "_" + str);
            for (int j = 0; j < size / 8; j++) {
                image = new(Calc.Random.Choose(atlasSubtextures));
                switch (Direction) {
                    case Directions.Up:
                        image.JustifyOrigin(0.5f, 1f);
                        image.Position = (Vector2.UnitX * (j + 0.5f) * 8f) + Vector2.UnitY;
                        break;
                    case Directions.Down:
                        image.JustifyOrigin(0.5f, 0f);
                        image.Position = (Vector2.UnitX * (j + 0.5f) * 8f) - Vector2.UnitY;
                        break;
                    case Directions.Right:
                        image.JustifyOrigin(0f, 0.5f);
                        image.Position = (Vector2.UnitY * (j + 0.5f) * 8f) - Vector2.UnitX;
                        break;
                    case Directions.Left:
                        image.JustifyOrigin(1f, 0.5f);
                        image.Position = (Vector2.UnitY * (j + 0.5f) * 8f) + Vector2.UnitX;
                        break;
                }

                Add(image);
            }
        }

        private void OnShake(Vector2 amount) {
            image.Position += amount;
        }

        private void OnCollide(Player player) {
            // Pretty sure this is unintentionally bugged due to short-circuiting, but the climb check could break existing maps if changed now?
            if (!(player.StateMachine.State == Player.StCassetteFly || player.StateMachine.State == Player.StBirdDashTutorial || player.StateMachine.State == Player.StClimb)) {
                switch (Direction) {
                    case Directions.Up:
                        if (player.Speed.Y >= 0f && player.Bottom <= Bottom) {
                            if (player.DashAttacking) {
                                player.Speed.Y = player.DashDir.X != 0 ? -260f : -320f;
                                player.DashDir.Y = -player.DashDir.Y;
                            } else {
                                player.Speed.Y = player.Speed.Y < 150f ? -180f : -1.2f * player.Speed.Y;
                            }

                            OnCertifiedHit(player.Position, player);
                            intoit = true;
                        }

                        break;
                    case Directions.Down:
                        if (player.Speed.Y <= 0f) {
                            player.Speed.Y = player.Speed.Y > -75f ? 90f : -1.2f * player.Speed.Y;
                            if (player.DashAttacking) {
                                player.DashDir.Y = -player.DashDir.Y;

                            }

                            OnCertifiedHit(player.Position, player);
                            intoit = true;
                        }

                        break;
                    case Directions.Left:
                        if (player.Speed.X >= 0f) {
                            if (player.DashAttacking && player.DashDir.X != 0) {
                                if (player.DashDir.Y < 0) {
                                    player.Speed.X = -1.4f * player.Speed.X;
                                    player.Speed.Y = 1.4f * player.Speed.Y;
                                    if (player.Speed.Y < 169) {
                                        player.Speed.Y = -237;
                                    }
                                } else {
                                    player.Speed.X = player.DashDir.Y > 0 ? -1.4f * player.Speed.X : -player.Speed.X;
                                }

                                player.DashDir.X = -player.DashDir.X;
                                if (player.Speed.X > -168) {
                                    player.Speed.X = -240;
                                }
                            } else {
                                player.Speed.X = player.Speed.X < 240f ? -240f : -1.2f * player.Speed.X;
                            }

                            OnCertifiedHit(player.Position, player);
                            intoit = true;
                        }

                        break;
                    case Directions.Right:
                        if (player.Speed.X <= 0f) {
                            if (player.DashAttacking && player.DashDir.X != 0) {
                                if (player.DashDir.Y < 0) {
                                    player.Speed.X = -1.4f * player.Speed.X;
                                    player.Speed.Y = 1.4f * player.Speed.Y;
                                    if (player.Speed.Y < 169) {
                                        player.Speed.Y = -237;
                                    }
                                } else {
                                    player.Speed.X = player.DashDir.Y > 0 ? -1.4f * player.Speed.X : -player.Speed.X;
                                }

                                player.DashDir.X = -player.DashDir.X;
                                if (player.Speed.X < 168) {
                                    player.Speed.X = 240;
                                }
                            } else {
                                player.Speed.X = player.Speed.X > -240f ? 240f : -1.2f * player.Speed.X;
                            }

                            OnCertifiedHit(player.Position, player);
                            intoit = true;
                        }

                        break;
                }
            }
        }

        private void OnHoldableCollide(Holdable holded) {
            Type type = holded.Entity.GetType();
            DynamicData dyn = new(type, holded.Entity);
            if (dyn.TryGet("Speed", out Vector2 theSpeed)) {
                switch (Direction) {
                    case Directions.Up:
                        if (theSpeed.Y >= 0f) {
                            if (theSpeed.Y < 150f) {
                                dyn.Set("Speed", new Vector2(theSpeed.X, -180f));
                            } else {
                                dyn.Set("Speed", new Vector2(theSpeed.X, -1.2f * theSpeed.Y));
                            }

                            OnCertifiedHoldableHit(holded.Entity.Position, holded);
                        }

                        break;
                    case Directions.Down:
                        if (theSpeed.Y <= 0f) {
                            if (theSpeed.Y > -75f) {
                                dyn.Set("Speed", new Vector2(theSpeed.X, 90f));
                            } else {
                                dyn.Set("Speed", new Vector2(theSpeed.X, -1.2f * theSpeed.Y));
                            }

                            OnCertifiedHoldableHit(holded.Entity.Position, holded);
                        }

                        break;
                    case Directions.Left:
                        if (theSpeed.X >= 0f) {
                            if (theSpeed.X < 240f) {
                                dyn.Set("Speed", new Vector2(-240f, theSpeed.Y));
                            } else {
                                dyn.Set("Speed", new Vector2(-1.2f * theSpeed.X, theSpeed.Y));
                            }

                            OnCertifiedHoldableHit(holded.Entity.Position, holded);
                        }

                        break;
                    case Directions.Right:
                        if (theSpeed.X <= 0f) {
                            if (theSpeed.X > -240f) {
                                dyn.Set("Speed", new Vector2(240f, theSpeed.Y));
                            } else {
                                dyn.Set("Speed", new Vector2(-1.2f * theSpeed.X, theSpeed.Y));
                            }

                            OnCertifiedHoldableHit(holded.Entity.Position, holded);
                        }

                        break;
                }
            }
        }

        public override void Update() {
            base.Update();
            Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
            if (player != null) {
                if (intoit) {
                    if (player.Speed.Length() < speed.Length() - 10) {
                        player.Speed = speed;
                    }

                    if (!CollideCheck<Player>()) {
                        if (dashedintoit) {
                            player.StateMachine.State = Player.StNormal;
                            dashedintoit = false;
                        }

                        if (!player.Inventory.NoRefills) {
                            player.RefillDash();
                        }

                        player.RefillStamina();
                        intoit = false;
                    }
                }

            }
        }

        private static int GetSize(EntityData data, Directions dir) {
            return dir == Directions.Left || dir == Directions.Right ? data.Height : data.Width;
        }

        private void OnCertifiedHit(Vector2 hitposition, Player player) {
            DynamicData playerData = new(player);
            if (FreezeFrameEnable) {
                Celeste.Freeze(0.03f);
            }

            if (player.DashAttacking == true) {
                playerData.Set("dashAttackTimer", 0f);
                playerData.Set("DashAttacking", false);
                OnCertifiedDash(hitposition);
            }

            Audio.Play("event:/char/badeline/jump_assisted");
            Emit4Particles(hitposition);
        }

        private void OnCertifiedHoldableHit(Vector2 hitposition, Holdable helded) {
            Audio.Play("event:/char/badeline/jump_assisted");
            Emit4Particles(hitposition);
        }

        private void OnCertifiedDash(Vector2 hitposition) {
            dashedintoit = true;
            SceneAs<Level>().Displacement.AddBurst(hitposition + particlePosAdjust, 0.4f, 8f, 64f, 0.5f, Ease.QuadOut, Ease.QuadOut);
        }

        private void Emit4Particles(Vector2 hitposition) {
            SceneAs<Level>().ParticlesFG.Emit(bounceParticle, hitposition + particlePosAdjust + particlePosAdjustTwo, particleAngle);
            SceneAs<Level>().ParticlesFG.Emit(bounceParticle, hitposition + particlePosAdjust - particlePosAdjustTwo, particleAngle + (float)(2 * Math.PI / 8));
            SceneAs<Level>().ParticlesFG.Emit(bounceParticle, hitposition + particlePosAdjust + particlePosAdjustTwo, particleAngle);
            SceneAs<Level>().ParticlesFG.Emit(bounceParticle, hitposition + particlePosAdjust - particlePosAdjustTwo, particleAngle + (float)(2 * Math.PI / 8));
        }

        private bool IsRiding(Solid solid) {
            return Direction switch {
                Directions.Up => CollideCheckOutside(solid, Position + Vector2.UnitY),
                Directions.Down => CollideCheckOutside(solid, Position - Vector2.UnitY),
                Directions.Left => CollideCheckOutside(solid, Position + Vector2.UnitX),
                Directions.Right => CollideCheckOutside(solid, Position - Vector2.UnitX),
                _ => false,
            };
        }

        private bool IsRiding(JumpThru jumpThru) {
            return Direction switch {
                Directions.Up => CollideCheck(jumpThru, Position + Vector2.UnitY),
                _ => false,
            };
        }
    }
}
