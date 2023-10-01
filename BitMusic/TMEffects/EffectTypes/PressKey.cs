using AutoHotkey.Interop;
using BitMusic.Settings;

namespace BitMusic.TMEffects.EffectTypes;

public class PressKey : EffectBase
{
    public string AhkKeyCode { get; }

    public PressKey(SettingsHandler settingsHandler, string displayName, bool enabled, uint weight, string ahkKeyCode) :
        base(settingsHandler, displayName, enabled, weight)
    {
        AhkKeyCode = ahkKeyCode;
    }

    private protected override void ExecuteRaw()
    {
        string code = $$"""
                        #IfWinActive ahk_exe {{TmSettings.ProcessName}}
                        Send , {{{AhkKeyCode}}}
                        """;

        AutoHotkeyEngine.Instance.ExecRaw(code);
    }
}
