using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Linq;

namespace Celeste.Mod.HonlyHelper {
    [Tracked]
    [CustomEntity("HonlyHelper/FriendKillTrigger")]
    class FriendKillTrigger : Trigger {
        public FriendKillTrigger(EntityData data, Vector2 offset)
            : base(data, offset) {
        }

        // Hackfix for Inwards (Holly map) which uses Isa's BadelineFollower
        // Long-term would be better to add a despawn method directly to IGB
        public override void OnEnter(Player player) {
            base.OnEnter(player);
            Follower follower = player.Leader.Followers.FirstOrDefault(f => f.Entity is BadelineDummy);
            if (follower != null) {
                player.Leader.LoseFollower(follower);
                follower.Entity.RemoveSelf();
                SceneAs<Level>().Session.SetFlag("has_badeline_follower", false);
            }
        }
    }
}
