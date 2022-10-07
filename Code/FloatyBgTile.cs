using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Celeste.Mod.HonlyHelper {
    [Tracked]
    [CustomEntity("HonlyHelper/FloatyBgTile")]
    public class FloatyBgTile : Entity {
        private static readonly MethodInfo FloatyAddToGroupAndFindChildren = typeof(FloatySpaceBlock).GetMethod("AddToGroupAndFindChildren", BindingFlags.Instance | BindingFlags.NonPublic);

        public List<FloatyBgTile> Group;
        public List<FloatySpaceBlock> Floaties;
        public Dictionary<Entity, Vector2> Moves;
        public Point GroupBoundsMin;
        public Point GroupBoundsMax;

        private readonly char tileType;
        private TileGrid tiles;
        private float yLerp;
        private float sinkTimer;
        private float sineWave;
        private float dashEase;
        private FloatyBgTile master;
        private bool awake;
        private bool HookedToFg;
        private FloatySpaceBlock HookedFg;

        public FloatyBgTile(Vector2 position, float width, float height, char tileType, bool disableSpawnOffset)
            : base(position) {
            this.tileType = tileType;
            sinkTimer = 0.3f;
            Depth = 10000;
            sineWave = !disableSpawnOffset ? Calc.Random.NextFloat((float)Math.PI * 2f) : 0f;
            Collider = new Hitbox(width, height);
            HookedToFg = false;
        }

        public FloatyBgTile(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Width, data.Height, data.Char("tiletype", '3'), data.Bool("disableSpawnOffset")) {
        }

        public bool HasGroup { get; private set; }
        public bool MasterOfGroup { get; private set; }

        public override void Awake(Scene scene) {
            base.Awake(scene);
            awake = true;
            if (!HasGroup) {
                MasterOfGroup = true;
                Moves = new Dictionary<Entity, Vector2>();
                Group = new List<FloatyBgTile>();
                GroupBoundsMin = new Point((int)X, (int)Y);
                GroupBoundsMax = new Point((int)Right, (int)Bottom);
                AddToGroupAndFindChildren(this);
                Rectangle rectangle = new(GroupBoundsMin.X / 8, GroupBoundsMin.Y / 8, ((GroupBoundsMax.X - GroupBoundsMin.X) / 8) + 1, ((GroupBoundsMax.Y - GroupBoundsMin.Y) / 8) + 1);
                VirtualMap<char> virtualMap = new(rectangle.Width, rectangle.Height, '0');
                foreach (FloatyBgTile item in Group) {
                    int num = (int)(item.X / 8f) - rectangle.X;
                    int num2 = (int)(item.Y / 8f) - rectangle.Y;
                    int num3 = (int)(item.Width / 8f);
                    int num4 = (int)(item.Height / 8f);
                    for (int i = num; i < num + num3; i++) {
                        for (int j = num2; j < num2 + num4; j++) {
                            virtualMap[i, j] = item.tileType;
                        }
                    }
                }

                tiles = GFX.BGAutotiler.GenerateMap(virtualMap, new Autotiler.Behaviour {
                    EdgesExtend = false,
                    EdgesIgnoreOutOfLevel = false,
                    PaddingIgnoreOutOfLevel = false
                }).TileGrid;
                tiles.Position = new Vector2(GroupBoundsMin.X - X, GroupBoundsMin.Y - Y);
                tiles.ClipCamera = SceneAs<Level>().Camera;
                tiles.VisualExtend = 1;
                Add(tiles);
            }

            TryToInitPosition();
        }

        public override void Update() {
            base.Update();

            if (MasterOfGroup) {
                if (sinkTimer > 0f) {
                    sinkTimer -= Engine.DeltaTime;
                }

                yLerp = sinkTimer > 0f ? Calc.Approach(yLerp, 1f, 1f * Engine.DeltaTime) : Calc.Approach(yLerp, 0f, 1f * Engine.DeltaTime);
                sineWave += Engine.DeltaTime;
                dashEase = Calc.Approach(dashEase, 0f, Engine.DeltaTime * 1.5f);
                MoveToTarget();
            }
        }

        public override void Removed(Scene scene) {
            tiles = null;
            Moves = null;
            Group = null;
            base.Removed(scene);
        }

        internal static void Load() {
            On.Celeste.FloatySpaceBlock.AddToGroupAndFindChildren += AddToGroupAndFindChildrenAddendum;
            On.Celeste.FloatySpaceBlock.Awake += AwakeAddendum;
            On.Celeste.FloatySpaceBlock.MoveToTarget += MoveToTargetAddendum;

        }
        internal static void Unload() {
            On.Celeste.FloatySpaceBlock.AddToGroupAndFindChildren -= AddToGroupAndFindChildrenAddendum;
            On.Celeste.FloatySpaceBlock.Awake -= AwakeAddendum;
            On.Celeste.FloatySpaceBlock.MoveToTarget -= MoveToTargetAddendum;
        }

        private static void AddToGroupAndFindChildrenAddendum(On.Celeste.FloatySpaceBlock.orig_AddToGroupAndFindChildren orig, FloatySpaceBlock self, FloatySpaceBlock from) {
            orig(self, from);
            DynamicData FloatySpaceBlockData = new(self);
            if (FloatySpaceBlockData.Get<List<FloatyBgTile>>("BgTileList") != null) {
                foreach (FloatyBgTile item3 in self.Scene.CollideAll<FloatyBgTile>(new Rectangle((int)from.X, (int)from.Y, (int)from.Width, (int)from.Height))) {
                    if (!FloatySpaceBlockData.Get<List<FloatyBgTile>>("BgTileList").Contains(item3)) {
                        if (!item3.awake) {
                            item3.Awake(self.Scene);
                        }

                        FloatySpaceBlockData.Get<List<FloatyBgTile>>("BgTileList").Add(item3);
                        FloatyBgTile item4 = item3;
                        if (item3.MasterOfGroup) {
                            item4 = item3;
                            item3.HookedFg = self;
                            item3.HookedToFg = true;

                        } else {
                            item4 = item3.master;
                            item3.master.HookedFg = self;
                            item3.master.HookedToFg = true;
                        }

                        foreach (FloatyBgTile floatyBoy in item4.Group) {
                            foreach (FloatySpaceBlock entity in self.Scene.Tracker.GetEntities<FloatySpaceBlock>()) {
                                if (!entity.HasGroup && self.Scene.CollideCheck(new Rectangle((int)floatyBoy.X, (int)floatyBoy.Y, (int)floatyBoy.Width, (int)floatyBoy.Height), entity)) {
                                    FloatyAddToGroupAndFindChildren.Invoke(self, new object[] { entity });
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void AwakeAddendum(On.Celeste.FloatySpaceBlock.orig_Awake orig, FloatySpaceBlock self, Scene scene) {
            if (!self.HasGroup) {
                DynData<FloatySpaceBlock> FloatySpaceBlockData = new(self);
                FloatySpaceBlockData["BgTileList"] = new List<FloatyBgTile>();
            }

            orig(self, scene);
        }

        private static void MoveToTargetAddendum(On.Celeste.FloatySpaceBlock.orig_MoveToTarget orig, FloatySpaceBlock self) {
            orig(self);
            DynData<FloatySpaceBlock> FloatySpaceBlockData = new(self);
            float num = (float)Math.Sin(FloatySpaceBlockData.Get<float>("sineWave")) * 4f;
            Vector2 vector = Calc.YoYo(Ease.QuadIn(FloatySpaceBlockData.Get<float>("dashEase"))) * FloatySpaceBlockData.Get<Vector2>("dashDirection") * 8f;
            float ylerpFloaty = FloatySpaceBlockData.Get<float>("yLerp");
            if (FloatySpaceBlockData.Get<List<FloatyBgTile>>("BgTileList") != null) {
                foreach (FloatyBgTile entity in FloatySpaceBlockData.Get<List<FloatyBgTile>>("BgTileList")) {
                    if (entity.MasterOfGroup) {
                        entity.MoveToTargetAttached(num, vector, ylerpFloaty);
                    }
                }
            }
        }

        private void TryToInitPosition() {
            if (MasterOfGroup) {
                foreach (FloatyBgTile item in Group) {
                    if (!item.awake) {
                        return;
                    }
                }

                MoveToTarget();
            } else {
                master.TryToInitPosition();
            }
        }

        private void AddToGroupAndFindChildren(FloatyBgTile from) {
            if (from.X < GroupBoundsMin.X) {
                GroupBoundsMin.X = (int)from.X;
            }

            if (from.Y < GroupBoundsMin.Y) {
                GroupBoundsMin.Y = (int)from.Y;
            }

            if (from.Right > GroupBoundsMax.X) {
                GroupBoundsMax.X = (int)from.Right;
            }

            if (from.Bottom > GroupBoundsMax.Y) {
                GroupBoundsMax.Y = (int)from.Bottom;
            }

            from.HasGroup = true;
            Group.Add(from);
            Moves.Add(from, from.Position);
            if (from != this) {
                from.master = this;
            }

            foreach (FloatyBgTile entity in Scene.Tracker.GetEntities<FloatyBgTile>()) {
                if (!entity.HasGroup && (Scene.CollideCheck(new Rectangle((int)from.X - 1, (int)from.Y, (int)from.Width + 2, (int)from.Height), entity) || Scene.CollideCheck(new Rectangle((int)from.X, (int)from.Y - 1, (int)from.Width, (int)from.Height + 2), entity))) {
                    if (from.HookedToFg) {
                        entity.HookedToFg = true;
                        entity.HookedFg = from.HookedFg;
                    }

                    AddToGroupAndFindChildren(entity);
                }
            }
        }

        private void MoveToTarget() {
            float num = (float)Math.Sin(sineWave) * 4f;
            Vector2 vector = Vector2.Zero;

            for (int i = 0; i < 2; i++) {
                foreach (KeyValuePair<Entity, Vector2> move in Moves) {
                    Entity key = move.Key;
                    Vector2 value = move.Value;
                    if (!HookedToFg) {
                        float num2 = MathHelper.Lerp(value.Y, value.Y + 12f, Ease.SineInOut(yLerp)) + num;
                        key.Position.Y = num2;
                        key.Position.X = value.X;
                    }
                }
            }
        }

        private void MoveToTargetAttached(float num, Vector2 vector, float ylerp) {
            for (int i = 0; i < 2; i++) {
                foreach (KeyValuePair<Entity, Vector2> move in Moves) {
                    Entity key = move.Key;
                    Vector2 value = move.Value;

                    float num2 = MathHelper.Lerp(value.Y, value.Y + 12f, Ease.SineInOut(ylerp)) + num;
                    key.Position.Y = num2 + vector.Y;
                    key.Position.X = value.X + vector.X;

                }
            }
        }
    }
}
