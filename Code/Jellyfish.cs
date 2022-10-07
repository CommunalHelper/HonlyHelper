using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste.Mod.HonlyHelper {
    [Tracked]
    [CustomEntity("HonlyHelper/Jellyfish")]
    public class Jellyfish : Creechure {
        public Player friend;
        public Vector2 homeAnchor;
        public Vector2 goalAnchor;

        private const float jellyMaxVelocity = 50f;

        private float floatTimer;
        private bool whenufloatin = false;

        private readonly Sprite jellySprite;
        private float angle;

        public Jellyfish(Vector2 position)
            : base(position) {
            friendSearchRadius = 56f;
            jellySprite = new Sprite(GFX.Game, "objects/HonlyHelper/Creechure/Jellyfish/");
            // fix sprites later
            jellySprite.AddLoop("Idle", "jelly", 0.15f, 0);
            Add(jellySprite);
            Collider = new Hitbox(16f, 16f, -8f, -8f);
            Add(new PlayerCollider(OnPlayer));
        }

        public Jellyfish(EntityData data, Vector2 offset)
            : this(data.Position + offset) {
        }

        public override void Awake(Scene scene) {
            if (!hasFriends) {
                friendJar = FindFriends<Jellyfish>(scene);
            }

            jellySprite.Play("Idle");
            homeAnchor = Position;
            jellySprite.Position = new Vector2(-8f, -8f);

            base.Awake(scene);
        }

        public override void Update() {
            floatTimer -= Engine.DeltaTime;
            if (floatTimer < 0 && !whenufloatin) {
                whenufloatin = true;
                Add(new Coroutine(jellyFlapFlap(UpdateMovementGoal(homeAnchor))));
            }

            base.Update();
        }

        public void OnPlayer(Player player) {
            friend = player;
            foreach (Jellyfish jellyFriend in friendJar) {
                jellyFriend.OnPlayerProxy(player);
            }
        }

        public void OnPlayerProxy(Player player) {
            friend = player;
        }

        private Vector2 UpdateMovementGoal(Vector2 anchor) {
            Vector2 movementGoal = anchor;
            if (friend != null) {
                movementGoal = friend.Position + new Vector2((Calc.Random.NextFloat() * 48f) - 24f, (Calc.Random.NextFloat() * 96f) - 48f);
            }

            return movementGoal;
        }

        public IEnumerator jellyFlapFlap(Vector2 goal) {
            float deltaX = goal.X - Position.X;
            float goalanglex = (float)(Math.Abs(deltaX) > 48 ? 0.25f * Math.PI : 0.25f * Math.PI * (Math.Abs(deltaX) / 48f)) * (deltaX > 0 ? 1 : -1);
            float rotationsize = Math.Abs(angle - goalanglex);

            while (Math.Abs(goalanglex - angle) > 0.1f) {
                angle = Calc.Approach(angle, goalanglex, rotationsize * 1f * Engine.DeltaTime);
                jellySprite.Rotation = angle;
                jellySprite.Position = new Vector2(12f * (float)Math.Sin(angle - (0.25f * Math.PI)), 12f * (float)Math.Cos(angle + (0.75f * Math.PI)));
                yield return null;
            }

            float launchSpeed = 0f;
            yield return null;

            while (launchSpeed < 0.95 * jellyMaxVelocity) {
                launchSpeed = Calc.Approach(launchSpeed, jellyMaxVelocity, 50 * Engine.DeltaTime);
                speed = (Vector2.UnitY * 16f) + (-Vector2.UnitY.Rotate(angle) * launchSpeed);
                yield return null;
            }

            while (launchSpeed > 0.02 * jellyMaxVelocity) {
                launchSpeed = Calc.Approach(launchSpeed, 0f, 25f * Engine.DeltaTime);
                speed = (Vector2.UnitY * 16f) + (-Vector2.UnitY.Rotate(angle) * launchSpeed);
                yield return null;
            }

            whenufloatin = false;
            floatTimer = (Calc.Random.NextFloat() * 0.5f) + 0.5f;
            yield break;
        }
    }
}
