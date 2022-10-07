using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste.Mod.HonlyHelper {
    public class FlyingBug : Actor {
        public float FlightStrength;
        public float MaxSpeed;
        public Color BugColor = Calc.HexToColor("ff0000");

        public Vector2 Speed = Vector2.Zero;
        public Vector2 OutdatedSpeed = Vector2.Zero;
        public Vector2 NewSpeed = Vector2.Zero;
        public Vector2 Acceleration = Vector2.Zero;
        public Vector2 FlightGoal;
        public float FlyError;

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

        public override void Update() {
            base.Update();
            FlyTowardsGoal(FlightGoal);
        }

        public override void Render() {
            Draw.Point(Position, BugColor);
            base.Render();
        }

        public virtual void FlyTowardsGoal(Vector2 Goal) {
            Vector2 toGoal = Goal - Position;
            float speeAngle = Calc.Angle(Speed);

            Vector2 speeGoal = Calc.AngleToVector(Calc.Angle(toGoal), FlightStrength);
            Acceleration = speeGoal - Speed;

            float accelerationlength = Acceleration.Length();
            accelerationlength = Math.Abs(accelerationlength) > FlightStrength ? FlightStrength : accelerationlength;
            float coolangle = speeAngle - Calc.Angle(Acceleration);

            float partwithLength = (float)Math.Cos(coolangle);
            partwithLength = partwithLength > 0f ? partwithLength * (1 - (Speed.LengthSquared() / MaxSpeed)) : partwithLength * (1 + (Speed.LengthSquared() / MaxSpeed));
            Vector2 partwith = Calc.AngleToVector(speeAngle, partwithLength * accelerationlength); // but then with speed trimming for spee cap
            Vector2 part90 = Calc.AngleToVector(speeAngle + (0.5f * (float)Math.PI), -(float)Math.Sin(coolangle) * accelerationlength);
            Acceleration = partwith + part90;

            Speed += Acceleration - (Speed * 0.1f * Engine.DeltaTime);
            Speed = Speed.Rotate(FlyError);
            NewSpeed = Vector2.Lerp(NewSpeed, OutdatedSpeed, 3f * Engine.DeltaTime);
            Position += NewSpeed * Engine.DeltaTime;
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

        private IEnumerator FlyUpdate() {
            while (true) {
                FlyError = (Calc.Random.NextFloat() - 0.5f) * 0.5f * (float)Math.PI;
                OutdatedSpeed = Speed;
                yield return (Calc.Random.NextFloat() * 0.05f) + 0.2f;
            }
        }
    }
}
