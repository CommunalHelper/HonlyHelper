using Microsoft.Xna.Framework.Input;

namespace Celeste.Mod.HonlyHelper {
    public class HonlyHelperSettings : EverestModuleSettings {
        [DefaultButtonBinding(Buttons.Y, Keys.B)]
        public ButtonBinding TalkToBadeline { get; set; } = new ButtonBinding();
    }
}