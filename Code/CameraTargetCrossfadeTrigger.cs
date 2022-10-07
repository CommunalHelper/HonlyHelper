using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System.Collections.Generic;

namespace Celeste.Mod.HonlyHelper {
    [Tracked]
    [CustomEntity("HonlyHelper/CameraTargetCrossfadeTrigger")]
    public class CameraTargetCrossfadeTrigger : Trigger {
        public Vector2 TargetA;
        public Vector2 TargetB;
        public float LerpStrengthA;
        public float LerpStrengthB;
        public PositionModes PositionMode;
        public bool XOnly;
        public bool YOnly;
        public string DeleteFlag;

        private readonly float LerpAToB;
        private Vector2 AToB;
        private float LerpPos;

        public CameraTargetCrossfadeTrigger(EntityData data, Vector2 offset)
            : base(data, offset) {
            TargetA = data.Nodes[0] + offset - (new Vector2(320f, 180f) * 0.5f);
            TargetB = data.Nodes[1] + offset - (new Vector2(320f, 180f) * 0.5f);
            LerpStrengthA = data.Float("lerpStrengthA");
            LerpStrengthB = data.Float("lerpStrengthB");
            PositionMode = data.Enum("positionMode", PositionModes.NoEffect);
            XOnly = data.Bool("xOnly");
            YOnly = data.Bool("yOnly");
            DeleteFlag = data.Attr("deleteFlag");

            AToB = TargetB - TargetA;
            LerpAToB = LerpStrengthB - LerpStrengthA;
        }

        public override void OnStay(Player player) {
            if (string.IsNullOrEmpty(DeleteFlag) || !SceneAs<Level>().Session.GetFlag(DeleteFlag)) {
                LerpPos = MathHelper.Clamp(GetPositionLerp(player, PositionMode), 0f, 1f);
                player.CameraAnchor = TargetA + (AToB * LerpPos);
                player.CameraAnchorLerp = Vector2.One * (LerpStrengthA + (LerpAToB * LerpPos));

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
