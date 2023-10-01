﻿using BitMusic.Settings;

namespace BitMusic.TMEffects.EffectTypes;

public class DummyEffect : EffectBase
{
    public DummyEffect(SettingsHandler settingsHandler) : base(settingsHandler, "Dummy Effect", false, 0)
    {
    }

    private protected override void ExecuteRaw()
    {
    }
}
