using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste.Mod.HonlyHelper {
    [Tracked]
    [CustomEntity("HonlyHelper/FireFly")]
    public class FireFly : FlyingBug {
        private const float BlinkTimerMax = 1.5f;
        private const float FriendFindRadius = 40f;
        private const float FriendInfluenceRadius = 64f;
        private const float RoamRadius = 24;

        private static readonly Color GlowColor = Calc.HexToColor("F7F440");
        private static readonly Color NormalColor = new(0.05f, 0.10f, 0.05f);

        private static int BugCount = 0;

        public float Brightness;
        public float BlinkTimer;
        public bool ControlsSelf;
        public bool BlinksSelf;
        public bool BlinkCancel = false;

        private readonly BloomPoint bloom;
        private Vector2 anchor;        
        private float ease;        
        private Vector2 longtermFlightGoal;
        private bool blinking = false;

        public FireFly(Vector2 position, bool ControlsSelf, bool BlinksSelf)
            : base(position, 40f, 100f) {
            Add(bloom = new BloomPoint(0.0f, 8f));
            anchor = position;
            this.ControlsSelf = ControlsSelf;
            this.BlinksSelf = BlinksSelf;
        }

        public FireFly(EntityData data, Vector2 offset)
            : this(data.Position + offset, true, true) {
        }

        public override void Awake(Scene scene) {
            if (BugCount == 0) {
                BugCount = scene.Tracker.CountEntities<FireFly>();
                if (BugCount > 3000) {
                    throw new Exception("please don't put over 3000 fireflies in a room, you put down " + BugCount + " fireflies, which is a crime, hand in your ahorn license immediately");
                }
            }

            if (ControlsSelf) {
                Add(new Coroutine(FireFlyUpdate()));
            }

            BlinkTimer = BlinkTimerMax * ((Calc.Random.NextFloat() * 0.5f) + 0.9f);
            BugColor = NormalColor;
            base.Awake(scene);
        }

        public override void Update() {
            BlinkTimer -= Engine.DeltaTime;
            if (BlinkTimer < 0 && !blinking) {
                blinking = true;
                Add(new Coroutine(Blink()));
            }

            BugColor = BrightnessToColour(Brightness);
            bloom.Alpha = Brightness;
            base.Update();
        }

        public IEnumerator Blink() {
            while (Brightness < 0.98f && !BlinkCancel) {
                ease = Calc.Approach(ease, 1f, Engine.DeltaTime * 8f);
                Brightness = Ease.CubeIn(ease);
                yield return null;
            }

            MakeSelfSeen();

            while (Brightness > 0.02 && !BlinkCancel) {
                ease = Calc.Approach(ease, 0f, Engine.DeltaTime * 1f);
                Brightness = Ease.QuadIn(ease);
                yield return null;
            }

            blinking = false;
            BlinkTimer = BlinkTimerMax * ((Calc.Random.NextFloat() * 0.5f) + 0.9f);
            BlinkCancel = false;
            yield return null;
        }

        private IEnumerator FireFlyUpdate() {
            while (true) {
                longtermFlightGoal = anchor + new Vector2(((2 * Calc.Random.NextFloat()) - 1) * RoamRadius, ((2 * Calc.Random.NextFloat()) - 1) * RoamRadius);
                while ((longtermFlightGoal - Position).LengthSquared() > 5) {
                    FlightGoal = Calc.Approach(FlightGoal, longtermFlightGoal, 10f * Engine.DeltaTime);
                    yield return null;
                }
                yield return (Calc.Random.NextFloat() * 0.5f) + 1f;
            }
        }

        private Color BrightnessToColour(float Bright) {
            return Color.Lerp(NormalColor, GlowColor, Bright);
        }

        private void SawBlink(float Distance) {
            if (BlinksSelf) {
                BlinkTimer *= Distance / ((float)Math.Pow(FriendInfluenceRadius, 2) + Distance);
            }
        }

        private void MakeSelfSeen() {
            if (BlinksSelf) {
                foreach (FireFly Fly in Scene.Tracker.GetWithinRadius<FireFly>(Position, FriendFindRadius)) {
                    if (Fly == null) {
                        break;
                    }

                    Fly.SawBlink(Vector2.DistanceSquared(Position, Fly.Position));
                }
            }
        }
    }
}
