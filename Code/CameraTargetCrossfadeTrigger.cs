using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste.Mod.HonlyHelper {
    [CustomEntity("HonlyHelper/CameraTargetCrossfadeTrigger")]
    [Tracked]
    public class CameraTargetCrossfadeTrigger : Trigger {
        public Vector2 TargetA;
        public Vector2 TargetB;

        public float LerpStrengthA;
        public float LerpStrengthB;

        public PositionModes PositionMode;

        public bool XOnly;

        public bool YOnly;

        public string DeleteFlag;

        private Vector2 AToB;
        private float LerpPos;
        private readonly float LerpAToB;

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
            bool flag = false;
            List<Entity> triggerList = new();

            triggerList.AddRange(Scene.Tracker.GetEntities<CameraAdvanceTargetTrigger>());
            triggerList.AddRange(Scene.Tracker.GetEntities<CameraTargetTrigger>());
            triggerList.AddRange(Scene.Tracker.GetEntities<CameraTargetCrossfadeTrigger>());
            triggerList.AddRange(Scene.Tracker.GetEntities<CameraTargetCornerTrigger>());

            foreach (Trigger trigger in triggerList) {
                if (trigger.PlayerIsInside) {

                    flag = true;
                    break;
                }
            }

            if (!flag) {
                player.CameraAnchorIgnoreX = false;
                player.CameraAnchorIgnoreY = false;
                player.CameraAnchorLerp = Vector2.Zero;
            }
        }
    }
}
