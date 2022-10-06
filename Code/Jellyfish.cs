using Microsoft.Xna.Framework;
using Monocle;
using System;
using Celeste.Mod.Entities;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;

namespace Celeste.Mod.HonlyHelper
{
    [Tracked(true)]
    [CustomEntity("HonlyHelper/Jellyfish")]
    public class Jellyfish : Creechure
    {

        public Player friend;
        public Vector2 homeAnchor;
        public Vector2 goalAnchor;

        private PlayerCollider pc;

        private const float jellyMaxVelocity = 50f;

        private float floatTimer;
        private bool whenufloatin = false;

        //private float friendSearchRadius = 56f;
        //public List<Jellyfish> jellyJar;
        //public bool hasFriends = false;

        private Sprite jellySprite;
        private Scene scene;
        private float angle;


        public Jellyfish(Vector2 position)
            : base(position)
        {
            friendSearchRadius = 56f;

            jellySprite = new Sprite(GFX.Game, "objects/HonlyHelper/Creechure/Jellyfish/");
            // fix sprites later
            jellySprite.AddLoop("Idle", "jelly", 0.15f, 0);
            Add(jellySprite);
            base.Solid = false;
            base.Collider = new Hitbox(16f, 16f, -8f, -8f);
            Add(pc = new PlayerCollider(OnPlayer));
        }

        public Jellyfish(EntityData data, Vector2 offset)
            : this(data.Position + offset)
        {
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            this.scene = scene;
        }

        public override void Removed(Scene scene)
        {
            base.Removed(scene);
        }

        public override void Awake(Scene scene)
        {
            if (!hasFriends)
            {
                friendJar = base.findFriends<Jellyfish>(scene);
            }
            jellySprite.Play("Idle");
            homeAnchor = Position;

            jellySprite.Position = new Vector2(-8f,-8f);

            base.Awake(scene);
        }

        public override void Render()
        {

            base.Render();
        }

        public override void Update()
        {
            floatTimer -= Engine.DeltaTime;
            if(floatTimer < 0 && !whenufloatin)
            {
                whenufloatin = true;
                this.Add(new Coroutine(jellyFlapFlap(updateMovementGoal(scene, homeAnchor))));
            }


            base.Update();
        }

        public override void OnPlayer(Player player)
        {
            friend = player;
            foreach (Jellyfish jellyFriend in friendJar){
                jellyFriend.OnPlayerProxy(player);
            }
        }

        public void OnPlayerProxy(Player player)
        {
            friend = player;
        }

        private Vector2 updateMovementGoal(Scene scene, Vector2 anchor)
        {
            Vector2 movementGoal = anchor;
            if(friend != null)
            {
                movementGoal = friend.Position + new Vector2(Calc.Random.NextFloat() * 48f - 24f, Calc.Random.NextFloat() * 96f - 48f);
            }
            /*
            movementGoal.Normalize();
            movementGoal *= 16f;
            /*
            foreach(Jellyfish jelly in scene.Tracker.GetEntities<Jellyfish>())
            {
                float distanceSquared = 0f;
                Vector2 jellyForce = Vector2.Zero;
                if (jelly != this)
                {
                    jellyForce = Vector2.Normalize(Position - jelly.Position);
                    distanceSquared = Vector2.DistanceSquared(jelly.Position, this.Position);
                    jellyForce *= 32f / (0.000000001f * (float)Math.Pow(distanceSquared, 3) + 1);
                    movementGoal += jellyForce;
                }
            }
            */

            return movementGoal;
        }

        public IEnumerator jellyFlapFlap(Vector2 goal)
        {
            float timercalculated = 0f;
            //the angling (acquire)
            float goalanglex = (goal - Position).Angle(); // kill later
            float deltaX = goal.X - Position.X;
            float goalangley = goalanglex + 0.5f * (float)Math.PI;
            if (goalangley > (float)Math.PI)
            {
                goalangley -= 2f*(float)Math.PI;
            }
            goalanglex = (Math.Abs(goalanglex)-0.5f) * (float)Math.PI; // garbage
            goalanglex = (goalanglex > (float)Math.PI / 3 ? (float)Math.PI / 3 : (goalanglex < -(float)Math.PI / 3 ? -(float)Math.PI / 3 : goalanglex));
            goalangley = (Math.Abs(goalangley)-0.5f) * (float)Math.PI;


            goalanglex = (float)(Math.Abs(deltaX) > 48 ? 0.25f * Math.PI : 0.25f * Math.PI * (Math.Abs(deltaX) / 48f)) * (deltaX > 0 ? 1 : -1);// - 0.5f * (float)Math.PI; // pog code
            //jellySprite.Rotation = goalanglex;
            //the angling (execute)
            float anglecopy = angle;
            float rotationsize = Math.Abs(angle - goalanglex);
            while (Math.Abs(goalanglex - angle) > 0.1f)
            {
                
                angle = Calc.Approach(angle, goalanglex, rotationsize * 1f * Engine.DeltaTime);
                jellySprite.Rotation = angle;
                jellySprite.Position = new Vector2(12f*(float)(Math.Sin(angle-0.25f*Math.PI)), 12f*(float)(Math.Cos(angle + 0.75f * Math.PI)));
                yield return null;
            }
            float launchSpeed = 0f;
            yield return null;
            //the launch
            while (launchSpeed <  0.95 * jellyMaxVelocity)
            {
                launchSpeed = Calc.Approach(launchSpeed, jellyMaxVelocity, 50 * Engine.DeltaTime);
                speed = Vector2.UnitY * 16f + -Vector2.UnitY.Rotate(angle)*launchSpeed;
                //player.Speed.X += 1;
                yield return null;
            }

            //the delay and floatback
            while (launchSpeed > 0.02 * jellyMaxVelocity)
            {
                launchSpeed = Calc.Approach(launchSpeed, 0f, 25f * Engine.DeltaTime);
                speed = Vector2.UnitY * 16f + -Vector2.UnitY.Rotate(angle) * launchSpeed;
                //player.Speed.X += 1;
                yield return null;
            }
            whenufloatin = false;
            floatTimer = Calc.Random.NextFloat() * 0.5f + 0.5f;
            yield break;
        }

        //moved to Creechure.cs
        /*
        public List<Jellyfish> findJellyFriends(Scene scene)
        {
            this.hasFriends = true;
            List<Jellyfish> friendsList = new List<Jellyfish>();
            foreach(Jellyfish jellyFriend in scene.Tracker.GetWithinRadius<Jellyfish>(this.Position, friendSearchRadius))
            {
                if (!jellyFriend.hasFriends)
                {
                    friendsList.Add(jellyFriend);
                    friendsList.AddRange(jellyFriend.findJellyFriends(scene));
                }
                
            }
            foreach(Jellyfish jellyFriend in friendsList)
            {
                jellyFriend.jellyJar = friendsList;
            }
            return friendsList;
        }
        */
    }
}