using Celeste;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using Celeste.Mod.Entities;


namespace Celeste.Mod.HonlyHelper
{
    public class HonlyHelperModule : EverestModule
    {
        public static HonlyHelperModule Instance { get; private set; }

        public HonlyHelperModule()
        {
            Instance = this;
        }

        public override Type SessionType => typeof(HonlyHelperSession);
        public override Type SettingsType => typeof(HonlyHelperSettings);
        public static HonlyHelperSettings Settings => (HonlyHelperSettings) Instance._Settings;
        public static HonlyHelperSession Session => (HonlyHelperSession) Instance._Session;

        public override void Load()
        {
            RisingBlock.Load();
            FloatyBgTile.Load();
        }
        public override void Unload()
        {
            RisingBlock.Unload();
            FloatyBgTile.Unload();
        }
        
    }
}