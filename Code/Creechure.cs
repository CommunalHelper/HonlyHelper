using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste.Mod.HonlyHelper {
    [Tracked(true)]
    public class Creechure : Entity {
        public List<Creechure> friendJar;
        public bool hasFriends = false;

        protected Vector2 speed;
        protected Vector2 movementGoal;
        protected float friendSearchRadius = 56f;

        public Creechure(Vector2 position)
            : base(position) {
        }

        public override void Update() {
            Position += speed * Engine.DeltaTime;
            base.Update();
        }

        protected List<Creechure> FindFriends<T>(Scene scene) where T : Creechure {
            hasFriends = true;
            List<Creechure> friendsList = new();
            foreach (T friend in scene.Tracker.GetWithinRadius<T>(Position, friendSearchRadius)) {
                if (!friend.hasFriends) {
                    friendsList.Add(friend);
                    friendsList.AddRange(friend.FindFriends<T>(scene));
                }
            }

            foreach (T friend in friendsList) {
                friend.friendJar = friendsList;
            }

            return friendsList;
        }
    }
}
