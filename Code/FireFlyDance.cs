using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste.Mod.HonlyHelper {
    [CustomEntity("HonlyHelper/FireFlyDance")]
    public class FireFlyDance : Entity {
        private readonly int[] blinkIndexes = { 0, 20, 40, 60, 80, 110, 140, 170 };
        private readonly List<FireFly> fireFlyList = new();
        private float sineTimer = 0f;
        private float vibeTimer = 0f;
        private bool vibin;
        private bool dancing;

        public FireFlyDance(Vector2 position)
            : base(position) {
            dancing = false;
            Collider = new Hitbox(500f, 260f, -250f, -130f);
            Add(new PlayerCollider(OnCollide));
        }

        public FireFlyDance(EntityData data, Vector2 offset)
            : this(data.Position + offset) {
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            for (int x = 0; x < 20; x++) {
                for (int y = 0; y < 10; y++) {
                    Vector2 pos = new Vector2(-240f + x * 24f, -120f + y * 24f) + Position;
                    FireFly fireFly = new(pos, true, true);
                    fireFlyList.Add(fireFly);
                    Scene.Add(fireFly);
                }
            }
        }

        public override void Update() {
            UpdateTimer();
            if (dancing) {
                int i = 0;
                int ringNumber = 0;
                float speedScalar = 1;
                foreach (FireFly FireFly in fireFlyList) {
                    if (i > 79) {
                        ringNumber = 1;
                        speedScalar = -1f;
                    }

                    FireFly.BlinksSelf = false;
                    FireFly.ControlsSelf = false;
                    float ringRadius = 56f + 32f * ringNumber;
                    Vector2 DancePosition = new(ringRadius * (float)Math.Sin(2 * (float)Math.PI * (i - 80 * ringNumber) / (ringNumber == 0 ? 80 : 120) + sineTimer * speedScalar), ringRadius * (float)Math.Cos(2 * (float)Math.PI * (i - 80 * ringNumber) / (ringNumber == 0 ? 80 : 120) + sineTimer * speedScalar));
                    FireFly.FlightGoal = Position + DancePosition;
                    FireFly.BlinkTimer = 10f;
                    i++;
                }
            }

            base.Update();
        }

        private void UpdateTimer() {
            sineTimer += Engine.DeltaTime * 0.4f;
            if (sineTimer > 2f * (float)Math.PI) {
                sineTimer -= 2f * (float)Math.PI;
            }

            if (!vibin) {
                if (vibeTimer > 0f) {
                    vibeTimer -= Engine.DeltaTime * 9f;
                } else {
                    dancing = false;
                }
            } else {
                if (vibeTimer < 180f) {
                    vibeTimer += Engine.DeltaTime;
                } else {
                    if (!dancing) {
                        dancing = true;
                        foreach (FireFly FireFly in fireFlyList) {
                            FireFly.BlinkCancel = true;
                        }

                        Add(new Coroutine(BlinkBlonkin()));
                    }
                }
            }

            vibin = false;
        }

        private IEnumerator BlinkBlonkin() {
            while (dancing) {
                for (int i = 0; i < blinkIndexes.Length; i++) {
                    fireFlyList[blinkIndexes[i]].Add(new Coroutine(fireFlyList[blinkIndexes[i]].Blink()));
                    fireFlyList[blinkIndexes[i]].BlinkTimer = 10f;
                    blinkIndexes[i] += (blinkIndexes[i] > 79 ? -1 : 1);
                    if (blinkIndexes[i] > 79 && i < (blinkIndexes.Length / 2)) {
                        blinkIndexes[i] -= 80;
                    } else if (blinkIndexes[i] < 80 && i > (blinkIndexes.Length / 2 - 1)) {
                        blinkIndexes[i] += 120;
                    }
                }

                yield return 0.025f;
            }

            yield return null;
        }

        private void OnCollide(Player player) {
            if (!vibin) {
                vibin = true;
            }
        }
    }
}
