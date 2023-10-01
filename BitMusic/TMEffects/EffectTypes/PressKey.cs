using BitMusic.Settings;
using BitMusic.TMEffects.EffectHelper;

namespace BitMusic.TMEffects.EffectTypes;

public class PressKey : EffectBase
{
    public string AhkKeyCode { get; }

    public PressKey(SettingsHandler settingsHandler, string displayName, bool enabled, uint weight, string ahkKeyCode) :
        base(settingsHandler, displayName, enabled, weight)
    {
        AhkKeyCode = ahkKeyCode;
    }

    public override void Execute()
    {
        string code = $$"""
                        #IfWinActive ahk_exe {{TmSettings.ProcessName}}
                        Send , {{{AhkKeyCode}}}
                        """;

        AhkHelper.ExecuteAhkScript(code);
    }
}
