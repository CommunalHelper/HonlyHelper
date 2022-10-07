using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.HonlyHelper {
    [Tracked]
    [CustomEntity("HonlyHelper/CameraTargetCornerTrigger")]
    public class CameraTargetCornerTrigger : Trigger {
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
                case PositionModeCorners.BottomRight:
                    ModX = 1;
                    break;
                case PositionModeCorners.TopLeft:
                    ModY = 1;
                    break;
                case PositionModeCorners.TopRight:
                    ModX = ModY = 1;
                    break;
                default:
                    break;
            }
        }

        public enum PositionModeCorners {
            BottomLeft,
            BottomRight,
            TopLeft,
            TopRight
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
            bool playerInAnchor = false;

            foreach (Trigger trigger in DynamicData.For(player).Get<HashSet<Trigger>>("triggersInside")) {
                if (trigger is CameraTargetTrigger or CameraAdvanceTargetTrigger or CameraTargetCornerTrigger or CameraTargetCrossfadeTrigger) {
                    playerInAnchor = true;
                    break;
                }
            }

            if (!playerInAnchor) {
                player.CameraAnchorIgnoreX = false;
                player.CameraAnchorIgnoreY = false;
                player.CameraAnchorLerp = Vector2.Zero;
            }
        }
    }
}
