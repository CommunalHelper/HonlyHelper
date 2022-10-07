using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste.Mod.HonlyHelper {
    public class FlyingBug : Actor {
        public Vector2 speed = Vector2.Zero;
        public Vector2 outdatedspeed = Vector2.Zero;
        public Vector2 newspeed = Vector2.Zero;
        public Vector2 acceleration = Vector2.Zero;
        public Vector2 FlightGoal;
        public float FlyError;
        public float FlightStrength;
        public float MaxSpeed;
        public Color BugColor = Calc.HexToColor("ff0000");

        public FlyingBug(Vector2 position, float FlightStrength, float MaxSpeed)
            : base(position) {
            this.FlightStrength = FlightStrength;
            this.MaxSpeed = (float)Math.Pow(MaxSpeed, 2);
            Depth = -20000;
        }

        public override void Awake(Scene scene) {
            FlightGoal = Position;
            Add(new Coroutine(FlyUpdate()));
            base.Awake(scene);
        }

        public override void Render() {
            Draw.Point(Position, BugColor);
            base.Render();
        }

        public override void Update() {
            base.Update();
            FlyTowardsGoal(FlightGoal);
        }

        private IEnumerator FlyUpdate() {
            while (true) {
                FlyError = (Calc.Random.NextFloat() - 0.5f) * 0.5f * (float)Math.PI;
                outdatedspeed = speed;
                yield return (Calc.Random.NextFloat() * 0.05f) + 0.2f;
            }
        }

        public virtual void FlyTowardsGoal(Vector2 Goal) {
            Vector2 toGoal = Goal - Position;
            float speeAngle = Calc.Angle(speed);

            Vector2 speeGoal = Calc.AngleToVector(Calc.Angle(toGoal), FlightStrength);
            acceleration = speeGoal - speed;

            float accelerationlength = acceleration.Length();
            accelerationlength = Math.Abs(accelerationlength) > FlightStrength ? FlightStrength : accelerationlength;
            float coolangle = speeAngle - Calc.Angle(acceleration);

            float partwithLength = (float)Math.Cos(coolangle);
            partwithLength = partwithLength > 0f ? partwithLength * (1 - (speed.LengthSquared() / MaxSpeed)) : partwithLength * (1 + (speed.LengthSquared() / MaxSpeed));
            Vector2 partwith = Calc.AngleToVector(speeAngle, partwithLength * accelerationlength); // but then with speed trimming for spee cap
            Vector2 part90 = Calc.AngleToVector(speeAngle + (0.5f * (float)Math.PI), -(float)Math.Sin(coolangle) * accelerationlength);
            acceleration = partwith + part90;

            speed += acceleration - (speed * 0.1f * Engine.DeltaTime);
            speed = speed.Rotate(FlyError);
            newspeed = Vector2.Lerp(newspeed, outdatedspeed, 3f * Engine.DeltaTime);
            Position += newspeed * Engine.DeltaTime;
        }

        public VertexLight GetNearestLight(Vector2 nearestTo) {
            List<Component> lights = Scene.Tracker.GetComponents<VertexLight>();
            VertexLight nearest = null;

            if (lights == null) //just return null??
            {
                return nearest;
            }

            float nearestDist = 0f;
            foreach (VertexLight light in lights) {
                float dist = Vector2.DistanceSquared(nearestTo, light.Entity.Position + light.Position);
                if ((nearest == null || dist < nearestDist) && !(light.Entity is Player || light.Entity is Actor)) {
                    nearest = light;
                    nearestDist = dist;
                }
            }

            return nearest;
        }
    }
}
