using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste.Mod.HonlyHelper {
    [Tracked]
    [CustomEntity("HonlyHelper/Moth")]
    public class Moth : FlyingBug {
        private VertexLight lamp;

        public Moth(Vector2 position)
            : base(position, 40f, 200f) {
            Add(new LightOcclude(new Rectangle(0, 0, 1, 1), 0.25f));
        }

        public Moth(EntityData data, Vector2 offset)
            : this(data.Position + offset) {
        }

        public override void Awake(Scene scene) {
            lamp = GetNearestLight(Position);
            Add(new Coroutine(MothUpdate()));
            BugColor = new Color((Calc.Random.NextFloat() * 0.1f) + 0.50f, (Calc.Random.NextFloat() * 0.10f) + 0.5f, (Calc.Random.NextFloat() * 0.10f) + 0.5f);
            base.Awake(scene);
        }

        public override void Update() {
            FlightGoal = lamp != null ? lamp.Entity.Position + lamp.Position : Position - (NewSpeed * Engine.DeltaTime * 20f);
            base.Update();
        }

        private IEnumerator MothUpdate() {
            while (true) {
                lamp = GetNearestLight(Position);
                yield return (Calc.Random.NextFloat() * 0.05f) + 0.2f;
            }
        }
    }
}
