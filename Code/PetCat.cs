using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste.Mod.HonlyHelper {
    [CustomEntity("HonlyHelper/PettableCat")]
    public class PettableCat : NPC {
        private readonly Sprite CatSprite;
        private readonly Sprite ThePetterSprite;
        private readonly string catFlag;
        private Coroutine pettingRoutine;
        private Vector2 CatAnchor;
        private bool PettingInProgress = false;
        private Vector2 friendposition;

        public PettableCat(Vector2 position, string CatFlag)
           : base(position) {
            CatSprite = new Sprite(GFX.Game, "characters/HonlyHelper/pettableCat/");
            CatSprite.AddLoop("idle", "spoons_idle", 0.15f);
            CatSprite.Add("pet", "spoons_pet", 0.15f);
            Add(CatSprite);

            ThePetterSprite = new Sprite(GFX.Game, "characters/HonlyHelper/pettableCat/ThePetter/");
            ThePetterSprite.AddLoop("idle", "player_idle", 1f);
            ThePetterSprite.AddLoop("pet", "player_pet", 0.15f);
            Add(ThePetterSprite);
            ThePetterSprite.Position = CatSprite.Position + new Vector2(-22f, -24f);

            catFlag = CatFlag ?? "CatHasBeenPet";
        }

        public PettableCat(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Attr("catFlag")) {
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            Add(Talker = new TalkComponent(new Rectangle(-30, 0, 64, 8), new Vector2(2f, -4f), OnPetting));
            CatAnchor = CatSprite.Position;
        }

        public override void Awake(Scene scene) {
            base.Awake(scene);
            CatSprite.Play("idle");
            ThePetterSprite.Play("idle");
        }

        private void OnPetting(Player player) {
            Level.StartCutscene(OnPettingEnd);
            Add(pettingRoutine = new Coroutine(ThePetting(player)));
        }

        private IEnumerator ThePetting(Player player) {
            yield return PlayerApproachLeftSide(player, turnToFace: true, 6f); 
            friendposition = player.Sprite.Position;
            PettingInProgress = true;
            CatSprite.Play("pet");
            CatSprite.Position = CatAnchor + new Vector2(-4f, -8f);
            ThePetterSprite.Play("pet");
            player.Sprite.Position = friendposition + (Vector2.UnitY * 1000f);
            Audio.Play("event:/HonlyHelper/catsfx", Center);
            yield return 2f;
            Level.EndCutscene();
            OnPettingEnd(Level);
            CatSprite.Play("idle");
            CatSprite.Position = CatAnchor;
            SceneAs<Level>().Session.SetFlag(catFlag);
        }

        private void OnPettingEnd(Level level) {
            Player entity = Scene.Tracker.GetEntity<Player>();
            if (entity != null) {
                entity.StateMachine.Locked = false;
                entity.StateMachine.State = Player.StNormal;
            }

            pettingRoutine.Cancel();
            pettingRoutine.RemoveSelf();
            CatSprite.Play("idle");
            CatSprite.Position = CatAnchor;
            if (PettingInProgress == true) {
                entity.Sprite.Position = friendposition;
            }

            entity.Sprite.Play("idle");
            ThePetterSprite.Play("idle");
            PettingInProgress = false;
        }
    }
}
