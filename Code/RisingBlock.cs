using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections;

namespace Celeste.Mod.HonlyHelper {
    [TrackedAs(typeof(FallingBlock))]
    [CustomEntity("HonlyHelper/RisingBlock")]
    public class RisingBlock : FallingBlock {
        private readonly DynamicData baseData;

        public RisingBlock(Vector2 position, char tile, int width, int height, bool finalBoss, bool behind, bool climbFall)
            : base(position, tile, width, height, finalBoss, behind, climbFall) {
            baseData = new DynamicData(typeof(FallingBlock), this);
        }

        public RisingBlock(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Char("tiletype", '3'), data.Width, data.Height, finalBoss: false, data.Bool("behind"), data.Bool("climbFall", defaultValue: true)) {
        }

        internal static void Load() {
            On.Celeste.FallingBlock.Sequence += FallingBlock_Sequence;
        }

        internal static void Unload() {
            On.Celeste.FallingBlock.Sequence -= FallingBlock_Sequence;
        }

        private static IEnumerator FallingBlock_Sequence(On.Celeste.FallingBlock.orig_Sequence orig, FallingBlock self) {
            if (self is RisingBlock block) {
                bool finalBoss = block.baseData.Get<bool>("finalBoss");

                while (!self.Triggered && (finalBoss || !block.baseData.Invoke<bool>("PlayerFallCheck"))) {
                    yield return null;
                }

                while (self.FallDelay > 0f) {
                    self.FallDelay -= Engine.DeltaTime;
                    yield return null;
                }

                while (true) {
                    block.baseData.Invoke("ShakeSfx");
                    self.StartShaking();
                    Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
                    if (finalBoss) {
                        self.Add(new Coroutine(block.baseData.Invoke<IEnumerator>("HighlightFade", new object[] { 1f })));
                    }

                    yield return 0.2f;
                    float timer = 0.4f;
                    if (finalBoss) {
                        timer = 0.2f;
                    }

                    while (timer > 0f && block.baseData.Invoke<bool>("PlayerWaitCheck")) {
                        yield return null;
                        timer -= Engine.DeltaTime;
                    }

                    self.StopShaking();
                    for (int i = 2; i < self.Width; i += 4) {
                        if (self.Scene.CollideCheck<Solid>(self.TopLeft + new Vector2(i, -2f))) {
                            self.SceneAs<Level>().Particles.Emit(P_FallDustA, 2, new Vector2(self.X + i, self.Y), Vector2.One * 4f, (float)Math.PI / 2f);
                        }

                        self.SceneAs<Level>().Particles.Emit(P_FallDustB, 2, new Vector2(self.X + i, self.Y), Vector2.One * 4f);
                    }

                    float speed = 0f;
                    float maxSpeed = finalBoss ? -130f : -160f;
                    while (true) {
                        Level level = self.SceneAs<Level>();
                        speed = Calc.Approach(speed, maxSpeed, 500f * Engine.DeltaTime);
                        if (self.MoveVCollideSolids(speed * Engine.DeltaTime, thruDashBlocks: true)) {
                            break;
                        }

                        if (self.Top > level.Bounds.Bottom + 16 || (self.Top > level.Bounds.Bottom - 1 && self.CollideCheck<Solid>(new Vector2(self.Position.X, self.Top - 1f)))) {
                            self.Collidable = self.Visible = false;
                            yield return 0.2f;
                            if (level.Session.MapData.CanTransitionTo(level, new Vector2(self.Center.X, self.Bottom + 12f))) {
                                yield return 0.2f;
                                self.SceneAs<Level>().Shake();
                                Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
                            }

                            self.RemoveSelf();
                            self.DestroyStaticMovers();
                            yield break;
                        }

                        yield return null;
                    }

                    block.baseData.Invoke("ImpactSfx");
                    Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
                    self.SceneAs<Level>().DirectionalShake(Vector2.UnitY, finalBoss ? 0.2f : 0.3f);
                    if (finalBoss) {
                        self.Add(new Coroutine(block.baseData.Invoke<IEnumerator>("HighlightFade", new object[] { 0f })));
                    }

                    self.StartShaking();
                    block.baseData.Invoke("LandParticles");
                    yield return 0.2f;
                    self.StopShaking();
                    if (self.CollideCheck<SolidTiles>(new Vector2(self.Position.X, self.Top - 1f))) {
                        break;
                    }

                    while (self.CollideCheck<Platform>(new Vector2(self.Position.X, self.Top - 1f))) {
                        yield return 0.1f;
                    }
                }

                self.Safe = true;
            } else {
                IEnumerator origEnum = orig(self);
                while (origEnum.MoveNext()) {
                    yield return origEnum.Current;
                }
            }
        }
    }
}
