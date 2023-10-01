using BitMusic.Settings;
using BitMusic.TMEffects.EffectHelper;

namespace BitMusic.TMEffects.EffectTypes;

public class HoldKey : PressKey
{
    public int ActiveTimeMs { get; }

    public HoldKey(SettingsHandler settingsHandler, string displayName, bool enabled, uint weight, string ahkKeyCode,
        int activeTimeMs) : base(settingsHandler, displayName, enabled, weight, ahkKeyCode)
    {
        ActiveTimeMs = activeTimeMs;
    }

    public override void Execute()
    {
        string code = $$"""
                        #IfWinActive ahk_exe {{TmSettings.ProcessName}}
                        Send , {{{AhkKeyCode}} down}
                        Sleep, {{ActiveTimeMs}}
                        Send , {{{AhkKeyCode}} up}
                        """;

        AhkHelper.ExecuteAhkScript(code);
    }
}
