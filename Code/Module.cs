using System;

namespace Celeste.Mod.HonlyHelper {
    public class HonlyHelperModule : EverestModule {
        public static HonlyHelperModule Instance { get; private set; }

        public HonlyHelperModule() {
            Instance = this;
        }

        public override Type SettingsType => typeof(HonlyHelperSettings);
        public static HonlyHelperSettings Settings => (HonlyHelperSettings)Instance._Settings;

        public override void Load() {
            RisingBlock.Load();
            FloatyBgTile.Load();
        }
        public override void Unload() {
            RisingBlock.Unload();
            FloatyBgTile.Unload();
        }
    }
}
