using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.HonlyHelper {
    [CustomEntity("HonlyHelper/CameraTargetCornerTrigger")]
    [Tracked]
    public class CameraTargetCornerTrigger : Trigger {
        public enum PositionModeCorners {
            BottomLeft,
            BottomRight,
            TopLeft,
            TopRight
        }

        public Vector2 Target;

        public float LerpStrength;

        public PositionModeCorners PositionMode;

        public bool XOnly;

        public bool YOnly;

        public string DeleteFlag;

        private readonly int ModX;
        private readonly int ModY;

        public CameraTargetCornerTrigger(EntityData data, Vector2 offset)
            : base(data, offset) {
            Target = data.Nodes[0] + offset - (new Vector2(320f, 180f) * 0.5f);
            LerpStrength = data.Float("lerpStrength");
            PositionMode = data.Enum("positionMode", PositionModeCorners.BottomLeft);
            XOnly = data.Bool("xOnly");
            YOnly = data.Bool("yOnly");
            DeleteFlag = data.Attr("deleteFlag");

            ModX = ModY = 0;

            switch (PositionMode) {
                case PositionModeCorners.BottomLeft:
                    break;
                case PositionModeCorners.BottomRight:
                    ModX = 1;
                    break;
                case PositionModeCorners.TopLeft:
                    ModY = 1;
                    break;
                case PositionModeCorners.TopRight:
                    ModX = ModY = 1;
                    break;
            }
        }

        public override void OnStay(Player player) {
            if (string.IsNullOrEmpty(DeleteFlag) || !SceneAs<Level>().Session.GetFlag(DeleteFlag)) {
                player.CameraAnchor = Target;
                player.CameraAnchorLerp = Vector2.One * (LerpStrength * Math.Abs(ModX - MathHelper.Clamp(GetPositionLerp(player, PositionModes.LeftToRight), 0f, 1f)) * Math.Abs(ModY - MathHelper.Clamp(GetPositionLerp(player, PositionModes.BottomToTop), 0f, 1f)));

                player.CameraAnchorIgnoreX = YOnly;
                player.CameraAnchorIgnoreY = XOnly;
            }
        }

        public override void OnLeave(Player player) {
            base.OnLeave(player);
            bool flag = false;
            foreach (CameraTargetTrigger entity in Scene.Tracker.GetEntities<CameraTargetTrigger>()) {
                if (entity.PlayerIsInside) {
                    flag = true;
                    break;
                }
            }

            if (!flag) {
                foreach (CameraAdvanceTargetTrigger entity2 in Scene.Tracker.GetEntities<CameraAdvanceTargetTrigger>()) {
                    if (entity2.PlayerIsInside) {
                        flag = true;
                        break;
                    }
                }
            }

            if (!flag) {
                foreach (CameraTargetCornerTrigger entity3 in Scene.Tracker.GetEntities<CameraTargetCornerTrigger>()) {
                    if (entity3.PlayerIsInside) {
                        flag = true;
                        break;
                    }
                }
            }

            if (!flag) {
                player.CameraAnchorIgnoreX = YOnly;
                player.CameraAnchorIgnoreY = XOnly;
                player.CameraAnchorLerp = Vector2.Zero;
            }
        }
    }
}
